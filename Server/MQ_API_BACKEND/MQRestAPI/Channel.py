class Channel:
    def __init__(self, channelName=None, channelType=None, description=None,
                 maxMessageLength=None, heartbeatInterval=None, transportType=None):
        self.channelName = channelName
        self.channelType = channelType
        self.description = description
        self.maxMessageLength = maxMessageLength
        self.heartbeatInterval = heartbeatInterval
        self.transportType = transportType

    def __str__(self):
        return str(self.__dict__)

    def to_dict(self):
        return {
            "channel_name": self.channelName,
            "channel_type": self.channelType,
            "description": self.description,
            "max_message_length": self.maxMessageLength,
            "heartbeat_interval": self.heartbeatInterval,
            "transport_type": self.transportType,
        }
