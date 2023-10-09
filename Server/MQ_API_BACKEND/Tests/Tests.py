import MQ

client = MQ.Client(url="https://13.87.80.195:9444", qmgr="QM1", username="admin", apikey ="passw0rd")

# base_url = "https://13.87.80.195:9444"
# username = "admin"
# password = "passw0rd"
def test():


    queues = client.get_all_queues()
    # config = PolicyConfiguration( queue_name_max_length=5)
    for q in queues:
        print(q.to_dict())



    # print(queues.to_dict())



test()
# if __name__ == '__main__':
#     execution_time = timeit.timeit(Test, number=1)
#     print("Execution time: {:.2f} seconds".format(execution_time))

