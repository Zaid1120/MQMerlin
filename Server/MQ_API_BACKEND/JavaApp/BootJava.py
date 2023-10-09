import subprocess
import os
import platform
from pathlib import Path


'''
This is a python file which will run the Java.

mvn spring-boot:run
'''

def set_env_variables(**properties):
    """
    Set environment variables from provided properties.
    """
    for key, value in properties.items():
        os.environ[key] = value


def start_spring_boot_app():
    """
    Start the Spring application.
    """
    # getting the path of the whole app
    script_directory = Path(__file__).parent
    base_directory = script_directory.parent.parent

    # java file
    jar_file = os.path.join(base_directory, "java", "MQ-listener.jar")
    cmd = ["java", "-jar", jar_file]


    # Redirect output to /dev/null to suppress it
    with open(os.devnull, 'w') as fnull:
        if platform.system() == "Windows":
            # On Windows, sometimes you need to use shell=True for subprocess.Popen
            process = subprocess.Popen(cmd, cwd=base_directory, stdout=fnull, stderr=fnull, shell=True)
        else:
            process = subprocess.Popen(cmd, cwd=base_directory, stdout=fnull, stderr=fnull)

    return process



def start_spring_app_with_properties(queue_manager, channel, conn_name, user, password, listener_auto_startup, event=None):
    """
    Accepts properties as arguments, sets them as environment variables,
    and starts the Spring Boot application.
    """
    set_env_variables(
        ibm_mq_queueManager=queue_manager,
        ibm_mq_channel=channel,
        ibm_mq_connName=conn_name,
        ibm_mq_user=user,
        ibm_mq_password=password,
        spring_jms_listener_auto_startup=listener_auto_startup
    )

    process = start_spring_boot_app()

    if event:
        event.set()

    return process



if __name__ == "__main__":
    start_spring_app_with_properties(
        queue_manager="QM2",
        channel="DEV.ADMIN.SVRCONN",
        conn_name="13.87.80.195(1415)",
        user="admin",
        password="passw0rd",
        listener_auto_startup="false"
    )
