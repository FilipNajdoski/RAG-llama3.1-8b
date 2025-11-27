Local RAG System (.NET 8 + React + Ollama)

A fully local Retrieval-Augmented Generation (RAG) system using:

Backend: ASP.NET Core 8 Web API

Frontend: React (Vite)

LLM: Ollama (llama3.1:8b)

Embeddings: nomic-embed-text

PDF support: UglyToad.PdfPig

This system runs completely offline and is free to use.




✅ SYSTEM REQUIREMENTS

Install these before running the project:

1. Node.js (Frontend)

Download and install:
https://nodejs.org

Verify install:

node -v
npm -v

2. .NET 8 SDK (Backend)

Download:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Verify:

dotnet --version

3. Ollama (Local AI engine)

Download and install:
https://ollama.com

Verify:

ollama --version

4. Pull Required AI Models
ollama pull llama3.1:8b
ollama pull nomic-embed-text

⚙️ BACKEND SETUP (.NET 8 API)

Install NuGet packages
dotnet add package UglyToad.PdfPig --prerelease

In the project RagBackend/Data/Documents folder >>> Add Example Knowledge File(.txt, .pdf)
Create:
Data/Documents/example.txt


🌐 FRONTEND SETUP (React + Vite)

Navigate:
cd RagFrontend

Install dependencies:
npm install

Run:
npm run dev

