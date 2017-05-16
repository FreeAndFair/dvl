The Digital Poll Book system
===

This project is part of the Verifiable Elections software
infrastructure, developed by Free & Fair. It is a *Digital
Poll Book System*, consisting of two main components: (1) a data
management application, which maintains voter lists, candidate lists,
and ballot design information and provisions digital poll books before
and during Election Day; and (2) a digital poll book application that
runs on commodity laptop or tablet devices to enable poll workers to
check in voters on Election Day.

Correctness of the software components, and security guarantees with
respect to the voter data it is used to manage, are critical aspects
of this project.

Prototype Background
====

The prototype implementations were developed as a part of the DemTech
Research Project at the IT University of Copenhagen in 2011-2013.

A *Digital Poll Book System* is also known as a *Digital Voter List
System*.  Consequently, the code name for this prototype is *DVL*.
The prototype was designed for use in Danish elections.

Development Process and Methodology
===

The *DVL*, including all prototypes and the current Aegis system, have
been developed using early variants of the DemTech Trust-by-Design
(TBD) software engineering methodology.

The TBD methodology is documented in several papers published by Joe
Kiniry and his coauthors, available via http://www.kindsoftware.com/.

In general, a system is comprised of:

* a top-level readme (like this one) that includes information about
  the system's purpose, examples of its use, fundamental concepts,
  system requirements, and background literature,

* a domain analysis and a detailed architecture specifications written
  in the Extended Business Object Notation (EBON)[3],

* formal specifications written at the source code level in one or
  more contract-based specification languages like [Code Contracts][1]
  (for .NET systems), the [Java Modeling Language][2] (for JVM
  systems), or the Executable ANSI/ISO C Specification Language
  (E-ACSL)[4],

* protocol descriptions typically formally specified using abstract
  state machines (ASMs), petri nets, formally annotated collaboration
  diagrams, or other formal notations that have tool support for
  reasoning about such protocols,

* a hand-written set of (sub)system tests and an automatically
  generated set of unit tests (using PEX[7] for .NET systems and
  JMLunitNG[8] for JVM ones), including reports on the completeness
  and quality of these validation artifacts, and

* a set of evidence that the system fulfills its requirements, usually
  in the form of traceable artifacts from the requirements to other
  artifacts that validate that they are satisfied (e.g., test results,
  code reviews, formal proofs, etc.).

Requirements
===

What follows are the mandatory and secondary requirements imposed upon
the *DVL*.  Informal verification (in the traditional software
engineering sense) of these requirements is accomplished by several
means, including formal verification of properties of the system's
specification and implementation, as well as traceability from the
requirements to artifacts that validate that they are satisfied (e.g.,
system tests, code review, etc.).

Mandatory Requirements
==

* Must be able to generate voter cards from a given set of eligible
  voters
* Must be able to authenticate a voter based on a voter card number
* Must be able to register when a voter has been handed a ballot and
  securely store this information
* Must prevent that a voter can be handed more than one ballot
* Must be able to authenticate and register voters at multiple
  machines simultaneously in various venues
* Must have an interactive user interface for authentication and
  registration
* Must be able to print out the current voterlist at any point of the
  election

Secondary Requirements
==

####Usability:

* The user interface must be trivial to use for non-technical users
  (election representatives).a

* The voter should be able to register at any table at the voting
  venue.

####Persistence:

* The system will exhibit no dataa lost from an arbitrary failure of
  any system in the *DVL* network (a typical system failure like a
  Windows crash).

* The system will exhibit no data loss in the event of a network
  failure.

####Scalability:

* The system should be able to handle a large number of voters
  (approximately 30,000 voter in a single voting venue with 10
  machines running the *DVL*).

####Security:

* The system should use proper security measures and cryptography to
  establish confidence that the system is secure.

* The system should be able to filter voters in a voter list based on
  multiple criterion to determine eligible voters.

* The system should be able to analyze the election result data to
  detect suspicious voters and fraud.

* The system should be able to provide a status on the digital voter
  list prior to an election and afterwards.

####Analysis:

* The system should be able to provide an analysis of the turnout,
  both nationally and for specific turnout results.

* The system should have a public API for the media or any citizen to
  access (after the election).

* The system should be able to visualize the turnout results.

* The system should print the list of eligible voters.

History
===

Several variants of the *DVL* were developed by around a dozen students
for end-of-term projects in the ITU course "Analysis, Design, and
Software Architecture with Project" under the supervision of Joe
Kiniry in Q4 2011.  These experiments are collected in the
"prototypes" directory in the repository.

The students that built these prototypes are Claes Martinsen, Niels
Martin Søholm Jensen, and Jan Aagaard Meier (P1); Emil Blædel Nygaard,
Michael Oliver Urhøj Mortensen, and Rasmus Greve (P2); Jens Dahl
Møllerhøj, Michael Valentin Erichsen, and Morten Hyllekilde Andersen
(P3); and Christian Olsson, Kåre Sylow Pedersen, and Henrik Haugbølle
(P4).  Each prototype was built in under one month using an early
version of the DemTech Trust-by-Design (TBD) methodology (see below).

Each prototype includes a short project overview, an architeture
specification, an implementation, and a validation suite.  Different
prototypes focused on different aspects of the problem.  E.g., some
focused on networking, others on crypography, etc.

In the following year (Q1-Q2 2012), two students, Nikolaj Aaes and
Nicolai Skovvart, developed an entirely new version of the *DVL* which
they called "the Aegis Digital Voter List" based upon what was learned
from the prototypes.  They used a refinement of TBD for their work and
spent six months developing the system.  Their system is meant to be
the foundation for a real, deployable, usable, fault tolerant, secure
*DVL*.

In early 2013 (Q1-Q2 2013), Morten Hyllekilde Andersen took over the
management of the project and made some technical contributions to it.
The intention of his work was to focus on election law and regulations
to ensure that we knew exactly what was necessary to do to prepare the
*DVL* for using in binding elections.

Starting in July 2013, Joe Kiniry took over development of the project
to push it to completion for using in 2013 elections.

Development Instructions
===

In order to develop on the prototype *DVL* the following software
libraries, tools, and frameworks must be installed.

[1]: Code Contracts library for .NET
[http://research.microsoft.com/en-us/projects/contracts/]
(http://research.microsoft.com/en-us/projects/contracts/)

[2]: Java Modeling Language (JML) 
[http://www.jmlspecs.org/](http://www.jmlspecs.org/)

[3]: The Business Object Notation
[http://bon-method.com/](http://bon-method.com/)

[4]: the Executable ANSI/ISO C Specification Language
[http://frama-c.com/](http://frama-c.com/)a

[5] The ADO.NET 2.0 Provider for SQLLite
[http://sourceforge.net/projects/sqlite-dotnet2/]
(http://sourceforge.net/projects/sqlite-dotnet2/)

  To install ADO.NET on a modern .NET 4.0 development system, you may
  need to install a .NET 3.5/2.0 runtime from Microsoft: 

[6] Adobe Acrobat Reader
[http://get.adobe.com/reader/](http://get.adobe.com/reader/)

[7] PEX
[http://research.microsoft.com/en-us/projects/pex/]
(http://research.microsoft.com/en-us/projects/pex/)

[8] JMLunitNG
[http://formalmethods.insttech.washington.edu/software/jmlunitng/]
(http://formalmethods.insttech.washington.edu/software/jmlunitng/)
