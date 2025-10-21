#!/bin/bash

TENANT_ID="ffdc3cc6-4bb5-41cc-a403-80cc927c43ab"

for i in {101..110}
do
  curl -s -X POST http://localhost:5101/api/tenants/$TENANT_ID/devices \
    -H "Content-Type: application/json" \
    -d "{ \"model\":\"Acme CO2-Temp\", \"serial\":\"dev-$i\", \"status\":\"active\" }"
  echo "" # ny rad mellan varje output
done
