import requests
agentGuid="edf3dc9f-06eb-43ab-b3ff-706acdfa5b11"
persist_path="../UserData/Agent/edf3dc9f-06eb-43ab-b3ff-706acdfa5b11/Memory.json"
def query_test():
    server_url = "http://localhost:8000/query"
    query = """
            Find the sequence of actions that will accomplish your goal according to the current states, return as an array.
            If you already has a plan and have a second action, try think step by step whether the first action is done and second action in the plan can be used, 
            if should be used then update the plan (remove the first action). If current plan can not fullfil the goal, make a new plan.

            I will give you the following information:

            Your current States:{"HasFood":false,"HasIngredients":false,"HasWater":false,"TargetIsHungry":false,"TargetIsDance":true,"HasEnergy":true,"IsThirsty":false}
            Your current Goal is:Follow Target and stand front of Target
            Your current Plan:[]
            Your current Action:Null

            You must follow the following criteria: 
            1. You should tell me in array format.
            ["a","b"]
            2. Only give me the array!
        """
    response = requests.post(server_url, json={"query": query,"guid": agentGuid})
    if response.status_code == 200:
        result = response.json()
        print(result)
    else:
        print("Error:", response.status_code, response.text)

def persist_test():
    server_url = "http://localhost:8000/persist"
    response = requests.post(server_url, json={"path": persist_path, "guid": agentGuid})
    if response.status_code == 200:
        result = response.json()
        print(result)
    else:
        print("Error:", response.status_code, response.text)

def initialize_test(key):
    server_url = "http://localhost:8000/initialize"
    response = requests.post(server_url, json={"apiKey":key})
    if response.status_code == 200:
        result = response.json()
        print(result)
    else:
        print("Error:", response.status_code, response.text)

if __name__ == "__main__":
    initialize_test("你的APIKey")
    persist_test()
    query_test()