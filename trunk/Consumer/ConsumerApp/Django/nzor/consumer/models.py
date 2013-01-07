from django.db import models

class Provider(models.Model):
    nzor_id = models.CharField(max_length=38)
    provider_name = models.CharField(max_length=100)
    url = models.URLField()
    
    def __unicode__(self):
        return self.provider_name
    
class Name(models.Model): 
    nzor_id = models.CharField(max_length=38)
    data_url = models.URLField()
    full_name = models.CharField(max_length=500)
    taxon_rank = models.CharField(max_length=100)
    authors = models.CharField(max_length=200)
    year = models.CharField(max_length=50)
    created_date = models.DateTimeField()
    modified_date = models.DateTimeField(null=True)
    parent_name = models.ForeignKey('self', related_name='name_parent_name',null=True)
    accepted_name = models.ForeignKey('self', related_name='name_accepted_name',null=True)
    classification = models.ManyToManyField('self',null=True)
    providers = models.ManyToManyField(Provider,null=True)
    
    def __unicode__(self):
        return self.full_name

class Reference(models.Model):
    nzor_id = models.CharField(max_length=38)
    data_url = models.URLField()
    citation = models.CharField(max_length=5000)
    created_date = models.DateTimeField()
    modified_date = models.DateTimeField(null=True)
    providers = models.ManyToManyField(Provider,null=True)
    
    def __unicode__(self):
        return self.citation
    
class Concept(models.Model):
    nzor_id = models.CharField(max_length=38)
    data_url = models.URLField()
    name = models.ForeignKey(Name)
    reference = models.ForeignKey(Reference)
    created_date = models.DateTimeField()
    modified_date = models.DateTimeField(null=True)
    providers = models.ManyToManyField(Provider,null=True)
    
    def __unicode__(self):
        return self.name
    
class Observation(models.Model):
    date_observed = models.DateTimeField()
    observer = models.CharField(max_length=200)
    locality = models.CharField(max_length=1000)
    decimal_latitude = models.FloatField(null=True)
    decimal_longitude = models.FloatField(null=True)
    verbatim_taxon = models.CharField(max_length=500)
    count = models.IntegerField(null=True)
    name_id = models.ForeignKey(Name, null=True)

    def __unicode__(self):
        return self.verbatim_taxon

class Harvest(models.Model):
    harvest_url = models.URLField()
    last_harvest_date = models.DateTimeField(null=True)
    
