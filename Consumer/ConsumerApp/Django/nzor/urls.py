from django.conf.urls.defaults import patterns, include, url
from django.contrib.staticfiles.urls import staticfiles_urlpatterns
from django.views.generic import DetailView, ListView, CreateView
from consumer.models import Name, Observation

# Uncomment the next two lines to enable the admin:
from django.contrib import admin
admin.autodiscover()

urlpatterns = patterns('',
    # Examples:
    # url(r'^$', 'consumer.views.home', name='home'),
    # url(r'^consumer/', include('consumer.foo.urls')),

    # Uncomment the admin/doc line below to enable admin documentation:
    # url(r'^admin/doc/', include('django.contrib.admindocs.urls')),

    # Uncomment the next line to enable the admin:
    (r'^consumer/$','consumer.views.index'),
    (r'^consumer/name/(?P<pk>\d+)/$',
         DetailView.as_view(model=Name, template_name='name.html')),
    (r'^consumer/observation/(?P<pk>\d+)/$',
         DetailView.as_view(model=Observation, template_name='observation.html')),
    (r'^consumer/observation/$',
         ListView.as_view(
             queryset=Observation.objects.all().order_by('-date_observed')[:10],
             context_object_name='obs_list',
             template_name='observationList.html')),
    (r'^consumer/name/$','consumer.views.name'),
    (r'^consumer/observation/add/$','consumer.views.observationAdd'),    
    url(r'^admin/', include(admin.site.urls)),
)

urlpatterns += staticfiles_urlpatterns()
