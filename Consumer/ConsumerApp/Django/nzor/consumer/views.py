from django.http import HttpResponse, HttpResponseRedirect
from django.shortcuts import render_to_response
from django.template import RequestContext
from obsForm import ObservationForm
from models import Observation
from harvesttask import harvest

def index(request):
    return HttpResponse("NZOR Example Consumer App")


def observationAdd(request):
    if request.method == 'POST': 
        form = ObservationForm(request.POST) 
        if form.is_valid():
            form.save()
            return HttpResponseRedirect('/consumer/observation/') 
    else:
        form = ObservationForm()

    return render_to_response('observationEdit.html', {
        'form': form,
    }, context_instance=RequestContext(request))
    
    
def name(request):
    if request.method == 'POST':
        harvest.delay()

    h = Harvest.objects.all()[0]
    return render_to_response('nameList.html', {
        'harvest': h,
    }, context_instance=RequestContext(request))
