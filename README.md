# Main Project

## Build all components locally

To build all the components locally, use the ```.\components\build-local.ps1``` to build.

## Run Locally

To run the Main Project locally, use the ```.\components\Up.ps1``` script to bring up the local environment.

## v1.0

v1.0 fufills all the requirement in the assignment. It using the test data from ```.\Data\example_data.json```. And output the correct results as requested in 3 tasks.

Once bringup the API in the docker container. Run the ConsoleApp in VS it will output the results.

## v2.0

Future Improvements:

- Load data via the backend service not via the API. Data will uploaded to AWS S3 bucket and when post the job to API it only contains the S3 URL. Backend service will download the file and load it.

- Add Put and Delete in the API

- Figure out a better way for complex searching and sorting

- Add authentication for the API something like OAuth
