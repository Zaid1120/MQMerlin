# DependencyGraph is constructed by iterating through the MQ entities
# such as queues, channels, and applications to extract (and sometimes infer)
# the "connectedness" amongst those entities.
#
# It is worth noting that the so-called "Dependency Graph" is not actually
# represented with a graph data strucutre, but rather dictionaries where
# the key is the name of the entity and the value is a list of entities
# that directly/indirectly relates to the key entity.

from MQRestAPI.Queues import RemoteQueue,  AliasQueue
from MQRestAPI.Channel import SenderChannel


class DependencyGraph:
    def __init__(self):
        self.direct_dependencies = {}
        self.indirect_dependencies = {}
        # Implicit Dependencies (i.e., the connectedness betweeen a sender and receiver channel pair)
        # is beyond the scope of the initial release. Consider for future implementation.
        self.implicit_dependencies = {} # For future implementation
        self.QM_NAME_DELIMITER = "."

    def to_dict(self):
        return {
            'direct_dependencies': self.direct_dependencies,
            'indirect_dependencies': self.indirect_dependencies,
            'implicit_dependencies': self.implicit_dependencies,
            'QM_NAME_DELIMITER': self.QM_NAME_DELIMITER
        }

    def create_dependency_graph(self, queues, channels, applications, qmgr):
        # Assuming MQ.Queue, MQ.Channel, MQ.Application etc are defined elsewhere
        for queue in queues:
            if isinstance(queue, AliasQueue):
                # Direct dependency of the alias queue
                direct_dependency = [qmgr + self.QM_NAME_DELIMITER + queue.target_queue_name]
                self.add_dependency(self.direct_dependencies, qmgr, queue.queue_name, direct_dependency)

                # Indirect dependency for the target queue of the alias queue
                indirect_dependency = [qmgr + self.QM_NAME_DELIMITER + queue.queue_name]
                self.add_dependency(self.indirect_dependencies, qmgr, queue.target_queue_name, indirect_dependency)

            if isinstance(queue, RemoteQueue):
                # Direct dependency of the remote queue
                direct_dependency = [qmgr + self.QM_NAME_DELIMITER + queue.transmission_queue_name,
                                     queue.target_qmgr_name + self.QM_NAME_DELIMITER + queue.target_queue_name]
                self.add_dependency(self.direct_dependencies, qmgr, queue.queue_name, direct_dependency)

                # Indirect dependency for the transmission queue and target queue of the remote queue
                indirect_dependency_trans = [qmgr + self.QM_NAME_DELIMITER + queue.queue_name]
                self.add_dependency(self.indirect_dependencies, qmgr, queue.transmission_queue_name,
                                    indirect_dependency_trans)

                indirect_dependency_target = [qmgr + self.QM_NAME_DELIMITER + queue.queue_name]
                self.add_dependency(self.indirect_dependencies, queue.target_qmgr_name, queue.target_queue_name,
                                    indirect_dependency_target)


        for channel in channels:
            if isinstance(channel, SenderChannel):
                # Direct dependency of sender channel
                direct_dependency = [qmgr + self.QM_NAME_DELIMITER + channel.transmission_queue_name]
                self.add_dependency(self.direct_dependencies, qmgr, channel.channel_name, direct_dependency)

                # Indirect dependency for the transmission queue of the sender channel
                indirect_dependency = [qmgr + self.QM_NAME_DELIMITER + channel.channel_name]
                self.add_dependency(self.indirect_dependencies, qmgr, channel.transmission_queue_name, indirect_dependency)

        for application in applications:
            connected_queues = application.get_connected_queues()
            for i in range(len(connected_queues)):
                # Indirect dependency for the connected queues of the application
                indirect_dependency_queue = [qmgr + self.QM_NAME_DELIMITER + application.conn]
                self.add_dependency(self.indirect_dependencies, qmgr, connected_queues[i], indirect_dependency_queue)

            # Indirect dependency for the application channel
            indirect_dependency_channel = [qmgr + self.QM_NAME_DELIMITER + application.conn]
            self.add_dependency(self.indirect_dependencies, qmgr, application.channel, indirect_dependency_channel)

            # Direct dependency for the application (here connectedQueues are the direct dependency)
            for i in range(len(connected_queues)):
                connected_queues[i] = qmgr + self.QM_NAME_DELIMITER + connected_queues[i]

            connected_queues.append(qmgr + self.QM_NAME_DELIMITER + application.channel)
            self.add_dependency(self.direct_dependencies, qmgr, application.conn, connected_queues)

    def clear_dependency(self):
        self.direct_dependencies.clear()
        self.indirect_dependencies.clear()
        self.implicit_dependencies.clear()

    def add_dependency(self, dependencies, qmName, entityName, dependency):
        key = qmName + self.QM_NAME_DELIMITER + entityName
        if key not in dependencies:
            dependencies[key] = dependency
        else:
            dependencies[key].extend(dependency)
