#!/bin/bash
export FLASK_APP=age_gender_estimation.py
python3 -m flask run --host=0.0.0.0 --port=7000
