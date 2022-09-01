# vac-seen-todb
Subscribes to "Vaccination" events in Kafka and persists them to a PostgreSQL database using Marten ([https://martendb.io/](https://martendb.io/)).

This is Part Six (of eight) of the C#, Kafka and OpenShift "vas-seen" system.

## Prerequisites and Requirements  
* You will need access to the command line, either Bash or PowerShell  
* You will need access to a web browser
* `oc` command line tool must be installed  
* You must have access to Red Hat OpenShift Sandbox
* The "vac-seen-db" project at https://github.com/donschenck/vac-seen-db must be completed.
* The "vac-seen-event-store project at https://github.com/donschenck/vac-seen-event-store must be completed.
* The "vac-seen-generator" project at https://github.com/donschenck/vac-seen-generator must be completed.
* The "vac-seen-managed-kafka" project at https://github.com/donschenck/vac-seen-managed-kafka must be completed.
* The "vac-seen-web" project at https://github.com/donschenck/vac-seen-web must be completed.


## Need help?
If you need help or get stuck, email devsandbox@redhat.com.
If you find a defect, [create an Issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/creating-an-issue) in this repository.

# Make sure events have been created
If you have not already completed Parts 1-5, you must do that in order to have events available in your Kafka instance to be processed by this activity.

# Create the application
This application is created as an OpenShift Job.

To create the job, run the following command:  

If using Bash, run:  
`./create-job.sh`

If using PowerShell, run:  
`./create-job.ps1` 

