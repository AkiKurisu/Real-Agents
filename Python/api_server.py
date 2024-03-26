import os
import uvicorn
from fastapi import FastAPI
from pydantic import BaseModel
from langchain.embeddings.openai import OpenAIEmbeddings
from langchain.vectorstores import Chroma
from langchain import OpenAI
from langchain.chains import RetrievalQA
from langchain.document_loaders import JSONLoader,DirectoryLoader
app = FastAPI()
# Set API
#os.environ["OPENAI_API_BASE"] = 'https://api.chatanywhere.tech/v1'
class QueryRequest(BaseModel):
    query: str
    guid: str
class PersistRequest(BaseModel):
    path: str
    guid: str
class InitializeRequest(BaseModel):
    apiKey: str
@app.post("/query")
def query_text(request: QueryRequest):
    embeddings = OpenAIEmbeddings()
    docsearch = Chroma(persist_directory=f"./vector_store/{request.guid}", embedding_function=embeddings)
    qa = RetrievalQA.from_chain_type(llm=OpenAI(), chain_type="stuff", retriever=docsearch.as_retriever(), return_source_documents=True)
    query = request.query
    result = qa(query)
    return result

@app.post("/persist")
def persist_memory(request: PersistRequest):
    print(f'Loading memory {request.path}')
    loader = JSONLoader(request.path,".actionMemories[]",text_content=False)
    documents = loader.load()
    embeddings = OpenAIEmbeddings()
    persistPath=f"./vector_store/{request.guid}"
    docsearch = Chroma.from_documents(documents, embeddings, persist_directory=persistPath)
    docsearch.persist()
    return {"message": "Persist memory completed successfully."}

@app.post("/persist/code")
def persist_memory(request: PersistRequest):
    print(f'Loading memory {request.path}')
    loader = DirectoryLoader(request.path, glob="**/*.txt")
    documents = loader.load()
    embeddings = OpenAIEmbeddings()
    persistPath=f"./vector_store/{request.guid}"
    docsearch = Chroma.from_documents(documents, embeddings, persist_directory=persistPath)
    docsearch.persist()
    return {"message": "Persist memory completed successfully."}

@app.post("/initialize")
def persist_memory(request: InitializeRequest):
    os.environ["OPENAI_API_KEY"]=request.apiKey
    return {"message": "OpenAI api key set successfully."}


if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)