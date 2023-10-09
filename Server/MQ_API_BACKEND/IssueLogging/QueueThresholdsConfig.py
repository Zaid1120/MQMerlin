from threading import Lock
import datetime

class QueueThresholdManager:
    def __init__(self):
        self._thresholds = {}
        self.defaultThreshold = 80 #(%)
        self._lock = Lock()

    def update(self, new_thresholds):
        with self._lock:
            self._thresholds.update(new_thresholds)

    def get(self, queue_name, default=None):
        with self._lock:
            return self._thresholds.get(queue_name, default)

    def contains(self, queue_name):
        with self._lock:
            return queue_name in self._thresholds

    def clear_thresholds(self):
        with self._lock:
            self._thresholds.clear()


    def thresholdWarning(self, queue, thresholdLimit):
        current_time = datetime.datetime.now().strftime('%Y-%m-%dT%H:%M:%S')  # ISO 8601 format
        alert_template = {
            "issueCode": "",
            "startTimeStamp": current_time,
            "generalDesc": "",
            "technicalDetails": {
                "maxThreshold": str(thresholdLimit)
            },
            "mqobjectType": "<QUEUE>",
            "mqobjectName": queue.queue_name,
            "objectDetails": str(queue)
        }

        if queue.current_depth == queue.max_number_of_messages:
            alert_template["issueCode"] = "Queue_Full"
            alert_template["generalDesc"] = "The queue is 100% full. Immediate action required!"
        elif queue.threshold >= thresholdLimit:
            alert_template["issueCode"] = "Threshold_Exceeded"
            alert_template[
                "generalDesc"] = f"The queue has exceeded the {thresholdLimit}% threshold limit. Please take necessary actions to avoid potential issues."
        else:
            return None

        return alert_template





