## What is this?

This is a reference API for creating metrics and exposing them in Prometheus format. A docker file is included to allow to containerise

## What's included

- net6 API
- Prometheus-net usage
- Wiring up event counter and meter metrics
- Collecting HTTP endpoint metrics
- Couple of examples of creating metrics from controllers
- Exposing the /metrics endpoint 
- A docker file to create an ima

# Weather Forecast API

This is a the default 'Weather Forecast API' implementation  with some metrics added - the purpose of this application is to demonstrate the `/metrics` endpoint that we want to expose for Prometheus. 

## Installation

Pull the source, then you can run in your IDE of choice to debug and explore. 

To build the image, from the root directory

``` bash
docker build .
docker run -d -p 80:80 {IMAGE-ID}
```

## Usage

When running a container, you can then access the endpoints at `http://localhost/api/WeatherForecast/Get` and `http://localhost/api/WeatherForecast/StormPrediction`. After accessing the endpoints, have a look at `http://localhost/metrics` to see the metrics collected

If running in your IDE, you'll have the Swagger UI to execute the endpoints. 

