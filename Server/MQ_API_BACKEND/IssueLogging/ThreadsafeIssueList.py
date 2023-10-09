import threading

class ThreadSafeIssueList:
    def __init__(self):
        self.issues = []
        self.lock = threading.Lock()

    def add_issue(self, issue):
        with self.lock:
            self.issues.append(issue)

    def get_issues(self):
        with self.lock:
            return self.issues.copy()

    def clear_issues(self):
        with self.lock:
            self.issues.clear()
