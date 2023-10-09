import requests
import time
import urllib3

# Suppress InsecureRequestWarning from urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

flask_endpoint = "https://127.0.0.1:5000/"

# #
address = "13.87.80.195"
admin_port = "9443"
app_port = "1414"
admin_channel = "DEV.ADMIN.SVRCONN"
qmgr = "QM1"
username = "admin"
password = "passw0rd"

class QMgrSystemReport:
    def __init__(self, qmanager_name, address, admin_port, app_port, admin_channel, username, password):
        self.qmanager_name = qmanager_name
        self.queues = []
        self.applications = []
        self.channels = []
        self.dependency_graph = {}
        self.post_client_config(address, admin_port, app_port, admin_channel, qmanager_name, username, password)


    def post_client_config(self, address, admin_port, app_port, admin_channel, qmgr, username, password):
        client_config = {
            "address": address,
            "admin_port": admin_port,
            "app_port": app_port,
            "admin_channel": admin_channel,
            "qmgr": qmgr,
            "username": username,
            "password": password
        }
        print(self.request_json("clientconfig", method="POST", data=client_config))


    def request_json(self, url, method="GET", data=None):
        try:
            if method == "GET":
                response = requests.get(flask_endpoint + url, verify=False)
            elif method == "POST":
                response = requests.post(flask_endpoint + url, json=data, verify=False)

            # Check HTTP response status here
            if response.status_code == 200:
                return response.json()
            elif response.status_code == 400:
                print(f"Bad request to {url}. Response: {response.json()}")  # Print out the server response
                return None
            else:
                print(f"Unexpected response from {url}. Status code: {response.status_code}")
                return None
        except requests.RequestException as e:
            print(f"Failed to fetch data from {url}. Issue: {e}")
            return None
        except ValueError:
            print(f"Invalid JSON response from {url}")
            return None

    def get_all_queues(self):
        response = self.request_json("getallqueues")
        if response:
            self.queues = response.get('All_Queues', [])
            print('Queues', self.queues)

    def get_all_applications(self):
        response = self.request_json("getallapplications")
        if response:
            self.applications = response.get('All_Applications', [])
            print('Applications', self.applications)

    def get_all_channels(self):
        response = self.request_json("getallchannels")
        if response:
            self.channels = response.get('All_Channels', [])
            print('Channels', self.channels)

    def get_dependency_graph(self):
        response = self.request_json("getdependencygraph")
        if response:
            self.dependency_graph = response.get('Dependency Graph', {})
            print('Dependency Graph', self.dependency_graph)

    def generate_report(self):
        print('Generating report for ', qmgr, "\n")
        self.get_all_queues()
        self.get_all_applications()
        self.get_all_channels()
        self.get_dependency_graph()

    def post_queue_thresholds(self, thresholds):
        """
        Posts threshold values to a given API endpoint.
        """
        response = self.request_json("queuethresholdmanager", method="POST", data=thresholds)
        if response:
            print('Thresholds Posted:', response)
        else:
            print("Failed to post queue thresholds.")

    def get_queue_thresholds(self):
        """
        Retrieves threshold values from a given API endpoint.
        """
        response = self.request_json("queuethresholdmanager", method="GET")
        if response:
            print('Retrieved Thresholds:', response)
        else:
            print("Failed to retrieve queue thresholds.")

    def get_issues(self):
        """
        Retrieves issues related to queue thresholds.
        """
        response = self.request_json("issues")
        if response:
            print('Issues:', response)
        else:
            print("Failed to retrieve issues.")

    def post_issue(self,issue_data):
        """
        Posts an error (or list of errors) to the server.
        """
        response = self.request_json("issues", method="POST",
                                data=issue_data)  # replace 'your_error_endpoint' with your actual endpoint
        print(response)

    def post_resolved_issue(self, mqobject_name, issue_code):
        """
        Posts a resolved issue to the server.
        """
        data = {
            "mqobjectName": mqobject_name,
            "issueCode": issue_code
        }
        response = self.request_json("resolve", method="POST", data=data)
        print(response)




def post_chatbot_query_and_get_response(query, indicator):
    def request_json(url, method="GET", data=None):
        try:
            if method == "GET":
                response = requests.get(flask_endpoint + url, verify=False)
            elif method == "POST":
                response = requests.post(flask_endpoint + url, json=data, verify=False)
            response.raise_for_status()
            return response.json()
        except requests.RequestException as e:
            print(f"Failed to fetch data from {url}. Issue: {e}")
            return None
        except ValueError:
            print(f"Invalid JSON response from {url}")
            return None

    # Step 1: Post the query and indicator to the chatbot endpoint
    data = {
        "question": query,
        "indicator": indicator
    }
    post_response = request_json("chatbotquery", method="POST", data=data)
    print(post_response)

    # Step 2: Upon confirmation, retrieve the chatbot response using a GET request
    if post_response and post_response.get("message") == "Query stored successfully.":
        time.sleep(1)  # Small delay to ensure server processes the query
        print('requesting response from chatbot')
        chatbot_response = request_json("chatbotquery")

        # Step 3: Return the chatbot response
        return chatbot_response

    print("Failed to retrieve chatbot response.")
    return None


def measure_execution_time(func):
    import time
    start_time = time.time()
    func()
    end_time = time.time()
    execution_time = end_time - start_time
    print("Execution time: {:.2f} seconds".format(execution_time))


# testing getting MQ API data
report_service = QMgrSystemReport(qmanager_name= qmgr, admin_channel=admin_channel, address=address, admin_port=admin_port, app_port=app_port, username= username, password=password)
# report_service.generate_report()


# testing posting threshold data
queue_threshold_config_payload = {
  "retrievedThresholds": {
    "apps": {
      "connThreshold": 100,
      "connOpRatioThreshold": 0.5,
      "minimumConns": 50
    },
    "queue_manager": {
      "errorThreshold": 10,
      "maxMQConns": 200,
      "maxMQOps": 1000
    },
    "queues": {
      "errorThreshold": 5,
      "queueThresholds": {
        "DEV.QUEUE.3": {
          "depth": 80,
          "activity": 200
        },
        "XMIT.TO.QM2": {
          "depth": 80,
          "activity": 200
        },
        "DEV.QUEUE.4": {
          "depth": 80,
          "activity": 200
        },
        "ORDER.RESPONSE": {
          "depth": 80,
          "activity": 200
        },
        "TEST.QUEUE.1": {
          "depth": 80,
          "activity": 200
        },
        "XMIT.TO.REMOTEQM": {
          "depth": 80,
          "activity": 200
        },
        "DEV.QUEUE.5": {
          "depth": 80,
          "activity": 200
        },
        "DEV.DEAD.LETTER.QUEUE": {
          "depth": 80,
          "activity": 200
        },
        "PUT.INHIBIT.QUEUE": {
          "depth": 80,
          "activity": 200
        },
        "DEV.QUEUE.1": {
          "depth": 80,
          "activity": 200
        },
        "MY.LOCAL.QUEUE": {
          "depth": 80,
          "activity": 200
        },
        "ORDER.REQUEST": {
          "depth": 80,
          "activity": 200
        },
        "INACCESSIBLE.QUEUE": {
          "depth": 80,
          "activity": 200
        },
        "XMIT.TO.QM3": {
          "depth": 80,
          "activity": 200
        },
        "DEV.QUEUE.2": {
          "depth": 80,
          "activity": 200
        },
        "queue1": {
          "depth": 10,
          "activity": 10
        },
        "queue2": {
          "depth": 20,
          "activity": 20
        }
      }
    }
  }
}

#
# # *****testing getting the threshold issue******
# # get another report so the issue will be triggered
report_service.generate_report()
# get threshold issue
error_data = [
    {
        "mqobjectType": "application",
        "mqobjectName": "38C9D06400090040",
        "issueCode" : "nuts"
    }
]

# Post the error data
report_service.post_issue(error_data)
mqobject_name = "DEV.QUEUE.5"
issue_code = "Threshold_Exceeded"  # Replace this with your actual issue code
report_service.post_resolved_issue(mqobject_name, issue_code)
mqobject_name = "38C9D06400090040"
issue_code = "nuts"  # Replace this with your actual issue code
report_service.post_resolved_issue(mqobject_name, issue_code)
report_service.get_issues()

report_service.get_queue_thresholds()
report_service.post_queue_thresholds(queue_threshold_config_payload)
report_service.get_queue_thresholds()





# ****testing chatbot*****:
# response = post_chatbot_query_and_get_response('{ "issueCode": "THRESHOLD_EXCEEDED", "startTimeStamp": "2023-09-04T14:25:30", "generalDesc": "The queue has exceeded the 80.0% threshold limit. Please take necessary actions to avoid potential issues.", "technicalDetails": { "maxThreshold": 0.8 }, "mqobjectType": "<QUEUE>", "mqobjectName": "DEV.QUEUE.2", "objectDetails": "QueueObjectRepresentationForDEV.QUEUE.2" }',  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("tell me about mq", "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("who should i contact for help on mq issues?",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("hi?",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("pits?",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("hello",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("who is obama",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("when elected?",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("u like giblets?",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("hi",  "userMessage")
# print(response)
# # response = post_chatbot_query_and_get_response("stains",  "userMessage")
# # print(response)
# response = post_chatbot_query_and_get_response("where are bananas red",  "userMessage")
# print(response)
# response = post_chatbot_query_and_get_response("tell me about mq",  "userMessage")
# print(response)



# ******threaded example******

# import threading
# import time
#
#
# def chatbot_query():
#     response = post_chatbot_query_and_get_response("what is a 2035 issue?", "systemMessage")
#     print(response)
#
#
# def generate_system_report():
#     for _ in range(5):  # Repeat 3 times
#         report_service.generate_reports()
#         time.sleep(10)  # Wait for 10 seconds between each report
#
#
# def example_usage():
#     # Start a new thread to handle the chatbot request
#     chatbot_thread = threading.Thread(target=chatbot_query)
#     chatbot_thread.start()
#
#     # Start a new thread to handle the MQ system report
#     system_report_thread = threading.Thread(target=generate_system_report)
#     system_report_thread.start()
#
#     # No need to wait for the chatbot thread in this function.
#     chatbot_thread.join()
#
#     # However, if you want to ensure that the main program doesn't exit until
#     # the system_report_thread is done, you can wait for it:
#     system_report_thread.join()
#
#
# # Now, execute the example usage
# example_usage()
