class QueueManager:
    def __init__(self):
        self.qmgr_name = None
        self.state = None

        # Populated by the State object
        self.queues = []
        self.channels = []
        self.applications = []

        # Extended
        self.installation_name = None
        self.permit_standby = None
        self.is_default_qmgr = None

        # Status
        self.publish_subscribe_state = None
        self.connection_count = None
        self.channel_initiator_state = None
        self.ldap_connection_state = None
        self.started = None

    def to_dict(self):
        return {
            'qmgr_name': self.qmgr_name,
            'state': self.state,
            'queues': [queue.to_dict() for queue in self.queues] if self.queues else [],
            'channels': [channel.to_dict() for channel in self.channels] if self.channels else [],
            'applications': [app.to_dict() for app in self.applications] if self.applications else [],
            'installation_name': self.installation_name,
            'permit_standby': self.permit_standby,
            'is_default_qmgr': self.is_default_qmgr,
            'publish_subscribe_state': self.publish_subscribe_state,
            'connection_count': self.connection_count,
            'channel_initiator_state': self.channel_initiator_state,
            'ldap_connection_state': self.ldap_connection_state,
            'started': self.started
        }
