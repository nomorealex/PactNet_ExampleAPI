# PactNet

# POC Backend Contract Testing

## Getting Started

In this repo you will find the resources related to the contract testing research.

Like:

- [X] Consumer application, simple HttpClient (Consumer directory)
- [X] Provider application, simple server made with: (Provider directory)
		- 2 GET HTTP methods;
		- 1 POST HTTP method;
		- 1 DELETE HTTP method.
- [X] Mock of the server, and generate the PACT file based on consumer[consumer-driven-testing] (ConsumerTests directory)
- [X] Verify Provider (ProviderTests directory)


## PACT (PACT-NET implementation)

See the Guide and Workflow for Pact-Net:
[PACT Setup]()






Run in Docker

```
docker run -v "C:\absolute\path\to\pact.json:/app/file.json pact-broker publish ...
```

```
docker-compose build
```

```
docker-compose up
```


1. Having two microservices a consumer and a provider
2. Run a mock of your provider
3. Generate the Pact file in the from the consumer
4. Then run the pact-broker with the docker-compose file
5. Then publish the pact to pact-broker
 ```
docker run -v C:\path\to\pacts\Consumer-Provider.json:/app/pact.json pactfoundation/pact-cli publish /app/pact.json --consumer-app-version 1 --broker-base-url="http://host.docker.internal:9292"
```
6. Then run the verify the providertest with dotnet test with the server running
7. Then use can-i-deploy to assure that a consumer and the provider are communicating correctly
