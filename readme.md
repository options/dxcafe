This application was developed during Korea DX ISV Team Hackfest of Microsoft Korea.
we developed applications using a variety of Microsoft technologies based on the intent of the event,
The scenarios for this application are:
DXCafe is a café that sells a variety of coffees.
Behind the existing ordering method, Now, the customer can order a coffee DXCafe kiosk system.
This automatically, recognize the user's face, and recommend the menu based on the previous order history.
When the day ends, the order list is delivered to the manager.

This application was developed using the following technologies:
Client Application: Developed based on WPF.
Face Recognition: Use Cognitive Service to distinguish users compared to previously registered faces.
Menu Recommendation: Uses the learned Azure ML expectation Model using the user's previous order information.
Ordering Information: Workflow is configured using Microsoft PowerApp and Microsoft Flow.
	