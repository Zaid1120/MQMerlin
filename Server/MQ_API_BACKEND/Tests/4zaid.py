import requests

def get_all_queues():
    endpoint = "http://127.0.0.1:5000/getallqueues"
    try:
        response = requests.get(endpoint)
        response.raise_for_status()
        queues = response.json().get('All_Queues', [])
        print('Queues', queues)
    except requests.RequestException as e:
        print(f"Failed to fetch data from {endpoint}. Issue: {e}")
    except ValueIssue:
        print(f"Invalid JSON response from {endpoint}")

get_all_queues()
