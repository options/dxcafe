# DXCafe : *Cognitive Kiosk* 
*This application was developed during Korea DX ISV Team Hackfest of Microsoft Korea.*

I have developed this applications using a variety of Microsoft technologies based on the intent of the event,

## The scenarios is:
* DXCafe is a café that sells a variety of coffees.
* Behind the existing ordering method, Now, the customer can order a coffee DXCafe kiosk system.
* This automatically, recognize the user's face, and recommend the menu based on the previous order history.
* When the day ends, the order list is delivered to the manager.

## This application was developed using the following technologies:
* Client Application: using **WPF**.
* Face Recognition: Use **Cognitive Service** to distinguish users face compared to previously registered faces.
* Menu Recommendation: Uses the learned **Azure ML** expectation Model using the user's previous order information.
* REST API Hosting : **Azure App Service** is used to execute Azure ML service
* Ordering Information: Workflow is configured using **Microsoft PowerApp** and **Microsoft Flow**.

## Screenshot

![Intro](https://github.com/options/dxcafe/blob/master/dxcafe1.png)
![Order](https://github.com/options/dxcafe/blob/master/dxcafe2.png)

## Azure ML Expectation Model
![Azure Model](https://github.com/options/dxcafe/blob/master/ml%20model.png)
