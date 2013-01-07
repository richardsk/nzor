from celery.task import task

@task
def harvest(url):
    print "here"
