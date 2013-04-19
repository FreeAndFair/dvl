Everyone is welcome to contribute to the development of the system. If you have any questions or comment, please write to the project admin: hyllekilde@demtech.dk

The Digital Voter List system
===
This project is developed as a part of the DemTech Research Project at the IT University of Copenhagen.

The project is about developing a Digital Voter List system that can be used for the danish election. It is a highly 
relevant project, as the municipalities are very interested in using a digital voter list, but the price for using the one provided by KMD is too high for many to afford.

Goal
===
The goal of the project is to make the DVL system as secure and complete as possible. It is made an Open Source project
in order to have as many assessments and thoughts made of the code and the way it is built which in the end will secure
the outcome.

Current priority of tasks
==
* Correction of bugs stated in 'issues'
* Implement the use of CryptDB
* 

Mandatory Requirements
===
* Must be able to generate voter cards from a given set of eligible voters
* Must be able to authenticate a voter based on a voter card number and a CPR number
* Must be able to register when a voter has been handed a ballot and store this information
* Must prevent that a voter can be handed more than one ballot
* Must be able to authenticate and register voters at multiple machines in various venues
* Must have an interactive user interface, for the authentication and registration
* Must be able to print out the current voterlist at any point of the election

Secondary Requirements
===

####Usability:
* Make the user interface easy to use for non-technical users (election representatives)
* The voter should be able to register at any table at the voting venue

####Persistence:
* Not loose data in the event of a typical system failure (eg. a computer crashes)
* Not loose data in the event of a network failure

####Scalability:
* The system should be able to handle a large number of voters. About 30.000 for each voting venue with 10 machines running the DVL

####Security:
* The system should use proper security measures and crypto-technology to establish confidence that the system is secure
* Filter the citizens based on multiple lists and criterion to determine eligible voters
* Analyze the resulting data, to detect suspicious voters and fraud
* Status on the digital voter list prior to election and after

####Analysis:
* Analysis of the turnout, both nationally and for specific turnout results
* A public API for the media or any citizen to access (after the election)
* Visualize the turnout results
* Print the list of eligible voters

Instructions
===
Coming up..
