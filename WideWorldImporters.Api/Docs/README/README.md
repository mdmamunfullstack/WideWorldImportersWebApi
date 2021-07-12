
# ASP.NET Core 5 REST Web API for SQL Server Wide World Importers database

## Table of contents
* [General Info](#general-info)
* [Technologies](#technologies)
* [Live API Tests with Swagger](#live-api-tests-with-swagger)
* [Postman Tests](#postman-tests)
* [Swagger](#swagger)
* [Class Diagram](#class-diagram)
* [Features](#features)

## General Info
This project uses C# to create a REST API for the Wide World Importers sample SQL Server database.

Resume:  [George McBath Resume](/George-McBath-Resume-346-452-9759.docx) (Word download)

## Technologies
Project is created with:
* ASP.NET Core 5 MVC
* C#

## Live API Tests with Swagger
You can use Swagger to test the live API at http://api.dotnetrestapi.com/swagger/index.html

## Postman Tests
In the root of the repository, the file named DotNetRestApi.postman_collection.json is a set of Postman requests.

## Class Diagram
For best results view with your favorite SVG program or look at raw file in browser.

![Class Diagram](/ClassDiagram.svg)

## Features
Features included with this project:
* ASP.NET Core 5 MVC
* Core 5 Entity Framework - synchronous and asynchronous
* Integration Tests
* DTOs to handle input and output requests
* Repository Pattern
* NLog
* Global Error Handler
* Content Negotiation 
* GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD
* Action Filters to facilitate code reuse
* Paging
* Keyword filtering
* Searching
* Sorting
* Data Shaping
* JSON, XML, CSV output serialization
* HATEOAS
* Custom Media Types
* Root Document
* API Versioning
* Caching
* Rate Limiting (currently rate limited to 10 requests every 2 minutes per IP - review x-rate-limit headers for details)
* Swagger API Documentation
{"mode":"full","isActive":false}