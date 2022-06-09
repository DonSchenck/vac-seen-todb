Write-Host "Gathering Service Binding to Kafka information..."

$SASL_MECHANISM=(invoke-expression "oc get kafkaconnections vaccinations -o json" | ConvertFrom-Json).status.metadata.saslMechanism
$SECURITY_PROTOCOL=(invoke-expression "oc get kafkaconnections vaccinations -o json" | ConvertFrom-Json).status.metadata.securityProtocol
$BOOTSTRAPSERVERS=(invoke-expression "oc get kafkaconnections vaccinations -o json" | ConvertFrom-Json).status.bootstrapServerHost
$SERVICE_ACCOUNT_SECRET_NAME=(invoke-expression "oc get kafkaconnections vaccinations -o json" | ConvertFrom-Json).status.serviceAccountSecretName

Write-Host "The following values will be used:"
Write-Host "------------------------------------------------------------------------------------------"
Write-Host "SASL_MECHANISM:             " $SASL_MECHANISM
Write-Host "SECURITY_PROTOCOL:          " $SECURITY_PROTOCOL
Write-Host "BOOTSTRAPSERVERS:           " $BOOTSTRAPSERVERS
Write-Host "SERVICE_ACCOUNT_SECRET_NAME:" $SERVICE_ACCOUNT_SECRET_NAME

oc process vac-seen-todb-job-template SERVICE_ACCOUNT_SECRET_NAME=$SERVICE_ACCOUNT_SECRET_NAME BOOTSTRAPSERVERS=$BOOTSTRAPSERVERS SASL_MECHANISM=$SASL_MECHANISM SECURITY_PROTOCOL=$SECURITY_PROTOCOL

invoke-expression "oc process vac-seen-todb-job-template SERVICE_ACCOUNT_SECRET_NAME=$SERVICE_ACCOUNT_SECRET_NAME BOOTSTRAPSERVERS=$BOOTSTRAPSERVERS SASL_MECHANISM=$SASL_MECHANISM SECURITY_PROTOCOL=$SECURITY_PROTOCOL" | oc create -f -