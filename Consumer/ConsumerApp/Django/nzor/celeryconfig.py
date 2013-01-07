BROKER_HOST = "localhost"
BROKER_PORT = 5672
BROKER_USER = "consumer"
BROKER_PASSWORD = "nzor"
BROKER_VHOST = "myvhost"

CELERY_RESULT_BACKEND = "amqp"
CELERY_RESULT_DBURI = "sqlite:///celerydb.sqlite"

CELERY_IMPORTS = ("harvesttask", )
