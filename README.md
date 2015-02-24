The Digital Poll Book System
===

This project is part of the Verifiable Elections software
infrastructure, developed by Verifiable Elections, Inc. It is a
*Digital Poll Book System*, consisting of two main components: (1) a 
data management application that imports voter and ballot data,
provisions digital poll books before and during Election Day, and
outputs voter roll updates after Election Day; and (2) a digital poll
book application that runs on commodity laptop or tablet devices to
enable voters to be checked in and presented with the appropriate
ballots on Election Day.

Correctness of these software components, and security guarantees with
respect to the voter data they handle, are critical aspects of this
project.

Prototype Background
===

The prototype implementations were developed as a part of the DemTech
Research Project at the IT University of Copenhagen in 2011-2013.

A *Digital Poll Book System* is also known as a *Digital Voter List
System*.  Consequently, the code name for this prototype is *DVL*.
The prototype was designed for use in Danish elections.

Development Process and Methodology
===

The *DVL*, including all prototypes and the current system, has been
developed using the Trust-by-Design (TBD) software engineering
methodology.

The TBD methodology is documented in several papers published by Joe
Kiniry and his coauthors, available via http://www.kindsoftware.com/.

In general, a system is comprised of:

* a top-level readme (like this one) that includes information about
  the system's purpose, examples of its use, fundamental concepts,
  system requirements, and background literature,

* a domain analysis and a detailed architecture specifications written
  in the [Extended Business Object Notation (EBON)] [3],

* formal specifications written at the source code level in one or
  more contract-based specification languages like [Code Contracts] [1]
  (for .NET systems), the [Java Modeling Language] [2] (for JVM
  systems), or the [Executable ANSI/ISO C Specification Language
  (E-ACSL)] [4],

* protocol descriptions typically formally specified using abstract
  state machines (ASMs), petri nets, formally annotated collaboration
  diagrams, or other formal notations that have tool support for
  reasoning about such protocols,

* a hand-written set of (sub)system tests and an automatically
  generated set of unit tests (using [PEX] [7] for .NET systems and
  [ JMLUnitNG] [8] for JVM ones), including reports on the completeness
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
* Must be able to register when a voter has been given a ballot and
  securely store this information
* Must prevent a voter from being given more than one ballot
* Must be able to authenticate and register voters at multiple
  machines simultaneously in various venues
* Must have an interactive user interface for authentication and
  registration
* Must be able to print out the current voter list at any point of the
  election

Secondary Requirements
==

####Usability:

* The user interface must be trivial to use for non-technical users
  (election representatives).
* The voter should be able to register at any table at the voting
  venue.

####Persistence:

* The system will exhibit no data loss from an arbitrary failure
  (e.g., a typical system failure like a Windows crash) of any system
  in the *DVL* network.
* The system will exhibit no data loss in the event of a network
  failure.

####Scalability:

* The system should be able to handle a large number of voters
  (approximately 30,000 voters in a single voting venue with 10
  machines running the *DVL*).

####Security:

* The system should use proper security measures and cryptography to
  establish confidence that the system is secure.
* The system should be able to filter voters in a voter list based on
  multiple criteria to determine eligible voters.
* The system should be able to provide sufficient audit information to
  allow the detection of suspicious voters and fraud.
* The system should be able to provide a status report on the digital
  voter list prior to an election and afterwards.

####Analysis:

* The system should be able to provide an analysis of the turnout,
  both nationally and for specific turnout results.
* The system should have a public API for the media or any citizen to
  access (after the election).
* The system should be able to visualize the turnout results.
* The system should be able to print the list of eligible voters.

History
===

Several variants of the *DVL* were developed by around a dozen
students for end-of-term projects in the ITU course "Analysis, Design,
and Software Architecture with Project" under the supervision of Joe
Kiniry in Q4 2011.  These experiments are collected in the
"prototypes" directory in the repository.

The students that built these prototypes are Claes Martinsen, Niels
Martin Søholm Jensen, and Jan Aagaard Meier (P1); Emil Blædel Nygaard,
Michael Oliver Urhøj Mortensen, and Rasmus Greve (P2); Jens Dahl
Møllerhøj, Michael Valentin Erichsen, and Morten Hyllekilde Andersen
(P3); and Christian Olsson, Kåre Sylow Pedersen, and Henrik Haugbølle
(P4).  Each prototype was built in under one month using an early
version of the Trust-by-Design (TBD) methodology (see above).

Each prototype includes a short project overview, an architecture
specification, an implementation, and a validation suite.  Different
prototypes focused on different aspects of the problem; e.g., some
focused on networking, others on crypography, etc.

In the following year (Q1-Q2 2012), two students, Nikolaj Aaes and
Nicolai Skovvart, developed an entirely new version of the *DVL* that
they called "the Aegis Digital Voter List", based upon what was learned
from the prototypes.  They used a refinement of TBD for their work and
spent six months developing the system.  Their system is meant to be
the foundation for a real, deployable, usable, fault tolerant, secure
*DVL*.

In early 2013 (Q1-Q2 2013), Morten Hyllekilde Andersen took over the
management of the project and made some technical contributions to it.
The intention of his work was to focus on election law and regulations
to ensure that we knew exactly what was necessary to prepare the
*DVL* for using in binding elections.

Starting in July 2013, Joe Kiniry took over development of the project
to push it to completion for use in 2013 elections.

Development Instructions
===

In order to develop on the prototype *DVL* the following software
libraries, tools, and frameworks must be installed.

[1]: http://research.microsoft.com/en-us/projects/contracts/  "Code Contracts library for .NET"

[2]: http://www.jmlspecs.org/  "Java Modeling Language (JML)"

[3]: http://bon-method.com/  "The Business Object Notation"

[4]: http://frama-c.com/ "The Executable ANSI/ISO C Specification Language"

[5]: http://sourceforge.net/projects/sqlite-dotnet2/ "The ADO.NET 2.0 Provider for SQLLite"

[6]: http://get.adobe.com/reader/ "Adobe Acrobat Reader"

[7]: http://research.microsoft.com/en-us/projects/pex/  "PEX"

[8]: http://formalmethods.insttech.washington.edu/software/jmlunitng/ "JMLUnitNG"
