from django.forms import ModelForm, ModelChoiceField, CharField, DecimalField, IntegerField
from django.forms.extras.widgets import SelectDateWidget
from django import forms 
from models import Observation, Name

class ObservationForm(ModelForm):
    class Meta:
        model = Observation
    name_id=ModelChoiceField(queryset=Name.objects.all(), required=False, empty_label="none")
    count=IntegerField(required=False)
    decimal_latitude=DecimalField(required=False)
    decimal_longitude=DecimalField(required=False)
