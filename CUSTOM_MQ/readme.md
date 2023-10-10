# Custom IBM MQ
This is an example of how to customise an IBM MQ container by changing configuration via '20-config.mqsc'

### Build
`docker build -t my-mq .`

### Default Configuration
The default options have been selected to match the MQ Docker container development configuration.

This means that you can run a queue manager using that Docker environment and connect to it. This script will run the container on a Linux system.

docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --publish 1414:1414 --publish 9443:9443 --detach --name QM1 my-mq

The default attributes are

ibm.mq.queueManager=QM1
ibm.mq.channel=DEV.ADMIN.SVRCONN
ibm.mq.connName=localhost(1414)
ibm.mq.user=admin
ibm.mq.password=passw0rd

### Extra configuration
Defined in the '20-config.mqsc' file

```
* Define custom queues
DEFINE QLOCAL('ORDER.REQUEST') REPLACE
DEFINE QLOCAL('ORDER.RESPONSE') REPLACE
```

### Mq Web console
Browse to the address
https://localhost:9443/ibmmq/console