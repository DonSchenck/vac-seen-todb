# vac-seen-todb
## What is this?
This repo is Part Six (of eight) of a workshop/activity/tutorial that comprises the "Vac-Seen System". This system is associated with, and specifically created for, the [Red Hat OpenShift Sandbox](https://developers.redhat.com/developer-sandbox).

At the end of this tutorial you will have an instance of a Kubernetes Job that is running in an OpenShift cluster. The job subscribes to "Vaccination" events in Kafka and persists them to a PostgreSQL database using Marten ([https://martendb.io/](https://martendb.io/)).


## Need help?
If you need help or get stuck, email devsandbox@redhat.com.
If you find a defect, [create an Issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/creating-an-issue) in this repository.

## Prerequisites and Requirements  
The following **ten** prerequisites are necessary:
1. An account in [OpenShift Sandbox](https://developers.redhat.com/developer-sandbox) (No problem; it's free). This is not actually *necessary*, since you can use this tutorial with any OpenShift cluster.
1. The `oc` command-line tool for OpenShift. There are instructions later in this article for the installation of `oc`.
1. Your machine will need access to a command line; Bash or PowerShell, either is fine.
1. You will need access to the command line, either Bash or PowerShell  
1. You will need access to a web browser
1. The "vac-seen-db" project at https://github.com/donschenck/vac-seen-db must be completed.
1. The "vac-seen-event-store project at https://github.com/donschenck/vac-seen-event-store must be completed.
1. The "vac-seen-generator" project at https://github.com/donschenck/vac-seen-generator must be completed.
1. The "vac-seen-managed-kafka" project at https://github.com/donschenck/vac-seen-managed-kafka must be completed.
1. The "vac-seen-web" project at https://github.com/donschenck/vac-seen-web must be completed.

## All Operating Systems Welcome
You can use this activity regardless of whether your PC runs Windows, Linux, or macOS.

# Make sure events have been created
If you have not already completed Parts 1-5 of the vac-seen system, you must do that in order to have events available in your Kafka instance to be processed by this activity.

# Create the application
This application is created as an OpenShift Job.

## Step 1: Create the job template
To create the job template, run the following command:  
`oc create -f vac-seen-todb-job-template.yaml`  

## Step 2: Create the Secret
The job uses a Secret object to connect to the database. Create the Secret by running the following command:  
`oc create -f vac-seen-todb-connection-string-secret.yaml`  

## Step 3: Create the job
Finally, create the job:  
If using Bash, run:  
`./create-job.sh`

If using PowerShell, run:  
`./create-job.ps1` 

