import requests
import json
from MQRestAPI.Queues import RemoteQueue, TransmissionQueue, AliasQueue, LocalQueue
from MQRestAPI.QMGR import QueueManager
from MQRestAPI.Channel import Channel
from MQRestAPI.Messages import Message
from MQRestAPI.Application import Application, ConnectedObject


class Client:
    def __init__(self, url, qmgr=None, username=None, password=None):
        self.baseUrl = url
        self.qmgr = qmgr
        self.username = username
        self.password = password
        self.session = requests.Session()
        self.session.headers.update({'Accept': 'application/json', 'ibm-mq-rest-csrf-token': 'value'})
        self.authenticate()

    def authenticate(self):
        request_url = f"{self.baseUrl}/ibmmq/rest/v1/login"
        body = json.dumps({"username": self.username, "password": self.password})

        try:
            response = self.session.post(request_url, data=body, headers={'Content-Type': 'application/json'},
                                         verify=False, timeout=5)
            response.raise_for_status()
        except requests.Timeout:
            raise Exception("Authentication request timed out after 5 seconds.")
        except requests.RequestException as e:
            raise Exception(f"Authentication request failed: {str(e)}")

    def get_queue_manager_name(self):
        return self.qmgr

    def get_request(self, endpoint):
        response = self.session.get(self.baseUrl + endpoint)
        response.raise_for_status()
        return response.text

    def post_request(self, endpoint, json_payload):
        response = self.session.post(self.baseUrl + endpoint, data=json_payload, headers={'Content-Type': 'application/json'})
        response.raise_for_status()
        return response.text

    def get_all_messages(self, queue):
        try:
            response = self.get_request(f"/ibmmq/rest/v1/messaging/qmgr/{self.qmgr}/queue/{queue}/messagelist")
            message_json = json.loads(response)
            messages = Parser.parse_message_response(message_json)
            return messages
        except requests.HTTPError as issue:
            if issue.response.status_code == 403:
                print("Issue 403: Forbidden. You do not have permission to get messages")
                return []
            else:
                raise  # re-raise the exception if it's not a 403 issue

    def get_message_content(self, queue, message_id):
        try:
            return self.get_request(
                f"/ibmmq/rest/v1/messaging/qmgr/{self.qmgr}/queue/{queue}/message?messageId={message_id}")
        except requests.HTTPError as issue:
            if issue.response.status_code == 403:
                print("Issue 403: Forbidden. You do not have permission to access this resource.")
                return None
            else:
                raise  # re-raise the exception if it's not a 403 issue

    def get_all_queue_managers(self):
        response = self.get_request(f"/ibmmq/rest/v1/admin/qmgr?attributes=*")
        all_qmgr_json = json.loads(response)
        qmgrs = Parser.parse_qmgr_response(all_qmgr_json)
        return qmgrs

    def get_qmgr(self):
        response = self.get_request(f"/ibmmq/rest/v1/admin/qmgr/{self.qmgr}?attributes=*")
        qmgr_json = json.loads(response)
        qmgrs = Parser.parse_qmgr_response(qmgr_json)
        return qmgrs[0]

    def get_all_queues(self):
        response = self.get_request(f"/ibmmq/rest/v1/admin/qmgr/{self.qmgr}/queue?attributes=*&status=*")

        queue_json = json.loads(response)

        queues = Parser.parse_queue_response(queue_json)

        # for q in queues:
        #     queue_messages = self.get_all_messages(q.queue_name)
        #     queue_messages_content_and_id = [{'Message ID': message.message_id,
        #                                       'Content': self.get_message_content(q.queue_name, message.message_id)}
        #                                      for message in queue_messages]
        #     q.messages = queue_messages_content_and_id

        return queues

    def get_queue(self, queue):
        response = self.get_request(f"/ibmmq/rest/v1/admin/qmgr/{self.qmgr}/queue/{queue}?attributes=*&status=*")
        queue_json = json.loads(response)
        queues = Parser.parse_queue_response(queue_json)
        return queues[0]



    def get_channel(self, channel):
        json_request = json.dumps({
            "type": "runCommandJSON",
            "command": "display",
            "qualifier": "channel",
            "name": channel,
            "responseParameters": ["all"],
            "parameters": {}
        })
        response = self.post_request(f"/ibmmq/rest/v2/admin/action/qmgr/{self.qmgr}/mqsc", json_request)
        channel_json = json.loads(response)
        channels = Parser.parse_channel_response(channel_json)
        return channels[0]

    def get_all_channels(self):
        json_request = json.dumps({
            "type": "runCommandJSON",
            "command": "display",
            "qualifier": "channel",
            "name": "*",
            "responseParameters": ["all"],
            "parameters": {}
        })
        response = self.post_request(f"/ibmmq/rest/v2/admin/action/qmgr/{self.qmgr}/mqsc", json_request)
        channels_json = json.loads(response)
        channels = Parser.parse_channel_response(channels_json)
        return channels

    def get_application(self, application):
        json_request = json.dumps({
            "type": "runCommandJSON",
            "command": "display",
            "qualifier": "conn",
            "name": application,
            "responseParameters": ["all"],
            "parameters": {
                "type": "*"
            }
        })
        response = self.post_request(f"/ibmmq/rest/v2/admin/action/qmgr/{self.qmgr}/mqsc", json_request)
        applications_json = json.loads(response)
        applications = Parser.parse_application_response(applications_json)
        return applications[0]

    def get_all_applications(self):
        json_request = json.dumps({
            "type": "runCommandJSON",
            "command": "display",
            "qualifier": "conn",
            "name": "*",
            "responseParameters": ["all"],
            "parameters": {
                "type": "*"
            }
        })
        response = self.post_request(f"/ibmmq/rest/v2/admin/action/qmgr/{self.qmgr}/mqsc", json_request)
        applications_json = json.loads(response)
        applications = Parser.parse_application_response(applications_json)  # modified this line
        filtered_applications = [application for application in applications if application.appltype != "SYSTEM"]
        return filtered_applications


class Parser:
    @staticmethod
    def parse_qmgr_response(qmgr_response_json):
        qmgrs = []
        for qmgr_json in qmgr_response_json['qmgr']:
            qmgr = QueueManager()
            qmgr.qmgr_name = qmgr_json['name']
            qmgr.state = qmgr_json['state']

            qmgr.installation_name = qmgr_json['extended']['installationName']
            qmgr.permit_standby = qmgr_json['extended']['permitStandby']
            qmgr.is_default_qmgr = qmgr_json['extended']['isDefaultQmgr']

            if 'status' in qmgr_json:  # Check if 'status' key is present
                qmgr.publish_subscribe_state = qmgr_json['status'].get('publishSubscribeState')
                qmgr.connection_count = qmgr_json['status'].get('connectionCount')
                qmgr.channel_initiator_state = qmgr_json['status'].get('channelInitiatorState')
                qmgr.ldap_connection_state = qmgr_json['status'].get('ldapConnectionState')
                qmgr.started = qmgr_json['status'].get('started')

            qmgrs.append(qmgr)

        return qmgrs

    @staticmethod
    def parse_queue_response(queue_response_json):
        queues = []

        for queue_json in queue_response_json['queue']:
            if "SYSTEM" in queue_json['name'] or "ADMIN" in queue_json['name']:
                continue

            queue_type = queue_json['type']
            if queue_type == 'alias':
                queue = AliasQueue()
                queue.target_queue_name = queue_json['alias']['targetName']
                queue.type_name = 'Alias'
                queue.inhibit_get = queue_json['general']['inhibitGet']
            elif queue_type == 'remote':
                queue = RemoteQueue()
                queue.target_queue_name = queue_json['remote']['queueName']
                queue.target_qmgr_name = queue_json['remote']['qmgrName']
                queue.transmission_queue_name = queue_json['remote']['transmissionQueueName']
                queue.type_name = 'Remote'
            elif queue_type == 'local':
                if queue_json['general']['isTransmissionQueue']:
                    queue = TransmissionQueue()
                    queue.current_depth = queue_json['status']['currentDepth']
                    queue.max_number_of_messages = queue_json['storage']['maximumDepth']
                    queue.max_message_length = queue_json['storage']['maximumMessageLength']
                    queue.time_created = queue_json['timestamps']['created']
                    queue.threshold = (queue.current_depth / queue.max_number_of_messages) * 100
                    queue.inhibit_get = queue_json['general']['inhibitGet']
                    queue.type_name = 'Transmission'
                else:
                    queue = LocalQueue()
                    queue.current_depth = queue_json['status']['currentDepth']  # might break
                    queue.max_number_of_messages = queue_json['storage']['maximumDepth']
                    queue.max_message_length = queue_json['storage']['maximumMessageLength']
                    queue.time_created = queue_json['timestamps']['created']
                    queue.threshold = (queue.current_depth / queue.max_number_of_messages) * 100
                    queue.inhibit_get = queue_json['general']['inhibitGet']
                    queue.type_name = 'Local'
            else:
                continue

            queue.queue_name = queue_json['name']
            queue.inhibit_put = queue_json['general']['inhibitPut']
            queue.description = queue_json['general']['description']
            queue.time_altered = queue_json['timestamps']['altered']


            queues.append(queue)


        return queues

    @staticmethod
    def parse_message_response(message_response_json):
        messages = []
        for message_json in message_response_json['messages']:
            message = Message()
            message.format = message_json['format']
            message.message_id = message_json['messageId']

            messages.append(message)
        return messages

    @staticmethod
    def parse_channel_response(channel_response_json):
        channels = []
        for channel_json in channel_response_json['commandResponse']:
            channel_params = channel_json['parameters']
            channelType = channel_params.get('chltype', "")

            # You can add any specific logic to handle different types of channels, e.g.:
            if channelType == 'AMQP':
                    continue

            channel = Channel()

            channel.channelName = channel_params.get('channel', "")
            channel.channelType = channelType
            channel.description = channel_params.get('desc', "")
            channel.maxMessageLength = channel_params.get('maxmsgl', "")
            channel.heartbeatInterval = channel_params.get('hbint', 0)
            channel.transportType = channel_params.get('trptype', "")

            channels.append(channel)

        return channels


    def parse_application_response(application_response_json):
        applications = []
        for application_json in application_response_json['commandResponse']:

            conn = application_json['parameters']['conn']
            channel = application_json['parameters']['channel']
            appltype = application_json['parameters']['appltype']
            appldesc = application_json['parameters']['appldesc']
            appltag = application_json['parameters']['appltag']
            conname = application_json['parameters']['conname']
            connopts = application_json['parameters']['connopts']
            conntag = application_json['parameters']['conntag']
            appl_objects = application_json['parameters']['objects'] if 'objects' in application_json['parameters'] else None

            connected_objects = []
            if appl_objects:
                for obj in appl_objects:

                    if 'openopts' in obj:
                        openopts = obj['openopts']
                    else:
                        openopts = None
                    connected_object = ConnectedObject(
                        obj['objname'],
                        obj['objtype'],
                        obj['hstate'],
                        obj['reada'],
                        openopts
                    )
                    connected_objects.append(connected_object)

            application = Application(
                conn=conn,
                channel=channel,
                type=appltype,
                conntag=conntag,
                conname=conname,
                connopts=connopts,
                appltype=appltype,
                appldesc=appldesc,
                appltag=appltag,
                connected_objects=connected_objects
            )

            applications.append(application)

        return applications




# #
# queues = client.get_all_queues()
# # config = PolicyConfiguration( queue_name_max_length=5)
# for q in queues:
#     print(q)
# ms = client.get_all_messages('DEV.QUEUE.3')
# print(ms)