# -*- coding: utf-8 -*-
"""
Created on Thu Mar  2 22:12:59 2023

@author: Ahmed
"""

import numpy as np
import pickle
import streamlit as st

# loading the saved model
loaded_model = pickle.load(open("D:/GradProjHeartAttackModel/trained_model.sav", 'rb'))


# creating a function for prediction


def heart_disease_prediction(input_data):
    # change the input data to a numpy array
    input_data_as_numpy_array = np.asarray(input_data)

    # reshape the numpy array as we are predicting for only on instance
    input_data_reshaped = input_data_as_numpy_array.reshape(1, -1)

    prediction = loaded_model.predict(input_data_reshaped)
    print(prediction)

    if prediction[0] == 0:
        return 'The Person does not have a Heart Disease'

    return 'The Person has Heart Disease'


def main():
    # The title
    st.title('Heart Disease Prediction Web App')

    # Getting the input data from the user

    age = st.text_input('The age')
    sex = st.radio("What\'s your sex", ('Female', 'Male'))
    if sex == 'Female':
        sex = 0
    elif sex == "Male":
        sex = 1
    cp = st.text_input('The Chest pain type')
    trtbps = st.text_input('The Resting Blood Pressure')
    chol = st.text_input('The cholesterol')
    fbs = st.text_input('The Fasting Blood Pressure')
    restecg = st.text_input('The Resting Electrocardiogram')
    thalachh = st.text_input('The Maximum Heart Rate Achieved')
    exng = st.text_input('The Exercise induced angina')
    oldpeak = st.text_input('The Previous peak')
    slp = st.text_input('The Slope')
    caa = st.text_input('The Number of major vessels')
    thall = st.text_input('The Thallium Stress Test result')

    # code for prediction
    diagnosis = ''

    # creating a button for prediction

    if st.button('Heart Disease Result'):
        diagnosis = heart_disease_prediction(
            [age, sex, cp, trtbps, chol, fbs, restecg, thalachh, exng, oldpeak, slp, caa, thall])

    st.success(diagnosis)


if __name__ == '__main__':
    main()
