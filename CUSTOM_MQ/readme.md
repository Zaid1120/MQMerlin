# Custom IBM MQ
This is an example of how to customise an IBM MQ container for monitoring by MQMerlin, changing configuration via '20-config.mqsc'

### Build
`docker build -t my-mq .`

### Default Configuration
The standard way to build an IBM MQ image.

docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --publish 1414:1414 --publish 9443:9443 --detach --name QM1 my-mq

The default attributes are

ibm.mq.queueManager=QM1
ibm.mq.channel=DEV.ADMIN.SVRCONN
ibm.mq.connName=localhost(1414)
ibm.mq.user=admin
ibm.mq.password=passw0rd

Which should be entered into MQMerlin's login page.


### Extra configuration
Defined in the '20-config.mqsc' file. Allows for events, accounting and statistics messages to be picked up by MQMerlin.
