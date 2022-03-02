# vac-seen-todb
Subscribes to "Vaccination" events in Kafka and persists them to a PostGreSQL database using Marten

Environment variable CONNECTION_STRING is to connect to the PostgreSQL database.

host=postgres;username=postgresql;password=7f986df431344327b52471df0142e520;

# Make sure events have been created
vac-seen-generator must have already run.

# Create objects
`oc create -f binding.yaml`  
`oc create -f vac-seen-todb-connection-string-secret.yaml`  
`oc create -f vac-seen-todb-job-template.yaml`  

# Create the job 
`create-job.ps1`  

# Observe the results
Use PostgreSQL commands to see the raw data.

kubectl exec deploy/postgresql -- /bin/bash ./tmp/query_marten.sh

psql -U username -d database.db -c "SELECT * FROM some_table"

                  List of relations
 Schema |          Name           | Type  |  Owner   
--------+-------------------------+-------+----------
 public | mt_doc_vaccinationevent | table | postgres

 oc exec mypod date

 oc exec -it postgresql-1-brr7z -- /usr/bin/psql -U postgres -d postgres -c 'select * from mt_doc_vaccinationevent'