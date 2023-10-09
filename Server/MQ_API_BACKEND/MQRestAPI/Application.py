# The Application class is used to parse the JSON response from
# the MQSC command "DISPLAY CONN"
# Reference: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=reference-display-conn-display-application-connection-information

class ConnectedObject:
    def __init__(self, objname=None, objtype=None, hstate=None, reada=None, openopts=None):
        self.objname = objname
        self.objtype = objtype
        self.hstate = hstate
        self.reada = reada
        self.openopts = openopts

    def __str__(self):
        return str(self.__dict__)

    def to_dict(self):
        return {
            "objname": self.objname,
            "objtype": self.objtype,
            "hstate": self.hstate,
            "reada": self.reada,
            "openopts": self.openopts
        }

class Application:
    def __init__(self, conn, channel, type, conntag, conname, connopts, appltype, appldesc, appltag, connected_objects):
        self.conn = conn
        self.channel = channel
        self.type = type
        self.conntag = conntag
        self.conname = conname
        self.connopts = connopts
        self.appltype = appltype
        self.appldesc = appldesc
        self.appltag = appltag
        self.connected_objects = connected_objects

    def get_connected_queues(self):
        connected_queues = []
        for conn in self.connected_objects:
            # There can be multiple types: QUEUE, TOPIC, QMGR
            # Look for QUEUE
            if conn.objtype == "QUEUE":
                connected_queues.append(conn.objname)
        return connected_queues

    def __str__(self):
        return str(self.__dict__)

    def to_dict(self):
        return {
            "conn": self.conn,
            "channel": self.channel,
            "type": self.type,
            "conntag": self.conntag,
            "conname": self.conname,
            "connopts": self.connopts,
            "appltype": self.appltype,
            "appldesc": self.appldesc,
            "appltag": self.appltag,
            "connected_objects": [obj.to_dict() for obj in self.connected_objects]
        }
