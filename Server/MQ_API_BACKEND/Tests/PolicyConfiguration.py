class PolicyConfiguration:
    def __init__(self, queue_name_case_sensitivity=True,
                 queue_name_max_length=48,
                 queue_name_min_length=2,
                 require_backup_queue=True,
                 max_number_of_messages=5000,
                 max_message_length=4194304,
                 message_depth_warning_threshold=0.9):

        self.queue_name_case_sensitivity = queue_name_case_sensitivity
        self.queue_name_max_length = queue_name_max_length
        self.queue_name_min_length = queue_name_min_length
        self.require_backup_queue = require_backup_queue
        self.max_number_of_messages = max_number_of_messages
        self.max_message_length = max_message_length
        self.message_depth_warning_threshold = message_depth_warning_threshold

    def validate_queue(self, queue):
        warnings = []

        if not self.queue_name_case_sensitivity and queue.queue_name != queue.queue_name.lower():
            warnings.append({
                'warning code': 'QueueNameCaseSensitivity',
                'issue message': "Queue name is not case-sensitive!",
                'queue': queue.queue_name
            })

        if len(queue.queue_name) > self.queue_name_max_length:
            warnings.append({
                'warning code': 'QueueNameMaxLen',
                'issue message': f"Queue name exceeds max length of {self.queue_name_max_length} characters!",
                'queue': queue.queue_name
            })

        if len(queue.queue_name) < self.queue_name_min_length:
            warnings.append({
                'warning code': 'QueueNameMinLen',
                'issue message': f"Queue name is less than min length of {self.queue_name_min_length} characters!",
                'queue': queue.queue_name
            })

        if queue.max_number_of_messages > self.max_number_of_messages:
            warnings.append({
                'warning code': 'MaxNumberOfMessages',
                'issue message': f"Queue exceeds max number of messages of {self.max_number_of_messages}!",
                'queue': queue.queue_name
            })

        if queue.max_message_length > self.max_message_length:
            warnings.append({
                'warning code': 'MaxMessageLen',
                'issue message': f"Queue message length exceeds max length of {self.max_message_length} bytes!",
                'queue': queue.queue_name
            })

        if self.require_backup_queue:
            pass  # Implement check when needed

        message_depth_ratio = queue.current_depth / queue.max_number_of_messages if queue.max_number_of_messages else 0
        if message_depth_ratio > self.message_depth_warning_threshold:
            warnings.append({
                'warning code': 'MessageDepthWarningThreshold',
                'issue message': f"Queue depth exceeds warning threshold of {self.message_depth_warning_threshold * 100}%!",
                'queue': queue.queue_name
            })

        return {'passed': len(warnings) == 0, 'Misconfiguration_Warnings': warnings}
