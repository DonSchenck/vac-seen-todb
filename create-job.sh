echo "Gathering Service Binding to Kafka information..."

export SASL_MECHANISM=$(oc get kafkaconnections vaccinations -o json | jq -r '.status.metadata.saslMechanism')
export SECURITY_PROTOCOL=$(oc get kafkaconnections vaccinations -o json | jq -r '.status.metadata.securityProtocol')
export BOOTSTRAPSERVERS=$(oc get kafkaconnections vaccinations -o json | jq -r '.status.bootstrapServerHost')
export SERVICE_ACCOUNT_SECRET_NAME=$(oc get kafkaconnections vaccinations -o json | jq -r '.status.serviceAccountSecretName')

echo "The following values will be used to create vac-seen-todb-cronjob:"
echo "------------------------------------------------------------------------------------------"
echo "SASL_MECHANISM:             " $SASL_MECHANISM
echo "SECURITY_PROTOCOL:          " $SECURITY_PROTOCOL
echo "BOOTSTRAPSERVERS:           " $BOOTSTRAPSERVERS
echo "SERVICE_ACCOUNT_SECRET_NAME:" $SERVICE_ACCOUNT_SECRET_NAME

oc process vac-seen-todb-cronjob-template SERVICE_ACCOUNT_SECRET_NAME=$SERVICE_ACCOUNT_SECRET_NAME BOOTSTRAPSERVERS=$BOOTSTRAPSERVERS SASL_MECHANISM=$SASL_MECHANISM SECURITY_PROTOCOL=$SECURITY_PROTOCOL

oc process vac-seen-todb-cronjob-template SERVICE_ACCOUNT_SECRET_NAME=$SERVICE_ACCOUNT_SECRET_NAME BOOTSTRAPSERVERS=$BOOTSTRAPSERVERS SASL_MECHANISM=$SASL_MECHANISM SECURITY_PROTOCOL=$SECURITY_PROTOCOL | oc create -f -