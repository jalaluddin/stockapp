Step 1) Steps to create and initialize the database
============================================================
Entity Framework Code First model has been used in this project so as long as all the connection strings are correct, there is no additional step required for initializing database, only 1 record needs to be inserted which is the API key for the client. Please follow e) element below to insert this record.

	a) Please create a new database with a proper name in Sql Server 2014. (i.e. StockMarketExpert)
	b) Please create a new database login with default schema dbo (i.e. jalal)
	c) Please assign the newly created user as the dbowner role for the newly created database. Make sure dbowner role is assigned.
	d) Please update the connection strings in the following path:
		i) Jalal Uddin - CSharpAssignment\Source\Crossover.TechTrial.StockExchange\StockExchangeAPI\Web.config
		ii) Jalal Uddin - CSharpAssignment\Source\Crossover.TechTrial.StockExchange\StockMarketApp\ConnectionString.config
	e) Please run the sql script in this folder: Jalal Uddin - CSharpAssignment\Source\Database in the newly created database.


Step 2) Steps to prepare the source code to build properly
============================================================
	a) Please make sure the solution is prepared for NuGet package restoration. Please check files under .neget exists, otherwise please right click the solution file in solution explorer and then activate nuget package restoration from context menu. It is mentioned as “Allow NuGet to Download missing packages” in context menu when you right click the solution file in solution explorer.
	b) Allow NuGet to download missing package: Make sure both the check box is checked under Visual Studio 2013 >  TOOLS > Options > NuGet Package Manager > General
	c) Double click the .sln file in location: Jalal Uddin - CSharpAssignment\Source\Crossover.TechTrial.StockExchange
	d) Apply build solution

Step 3) Any assumptions made and missing requirements that are not covered in the requirements
==============================================================================================
There was not enough clarification about meaning of the web service security. Whether it was meant that if the service is called by unauthorized client, then the service will not process the request or the service will not be visible to public internet was not clear enough. I have assumed that by securing it, I was asked to make sure if unauthorized user calls the service, the service will not process the request. Because I think the option for denying unintended client can be more easily configured in newtwork level rather than in coding level, so restricting request processing is what was meant here - I assumed this.

Step 4) Nice to have: Steps to install the service and web application on IIS using the deploy packages
=======================================================================================================
Two web deployment packages for the two projects has been created in the following directory:
	i) Jalal Uddin - CSharpAssignment\Source\Crossover.TechTrial.StockExchange\DeployPackages\StockMarketApp
	ii) Jalal Uddin - CSharpAssignment\Source\Crossover.TechTrial.StockExchange\DeployPackages\StockExchangeAPI
Each of these directories contains a readme file (i.e. StockExchangeAPI.deploy-readme.txt) which contains instruction to deploye the project in IIS

Additionally the StockMarketApp and StockExchangeAPI projects can be published manually by right clicking the project file and selecting Publish from context menu. The options for various publish media is self explanatory.

Step 5) Any feedback you may wish to give about improving the assignment.
=========================================================================
I enjoyed working in the assignment. It is a nice way to measure skill. I feel unit tests can be included in the task. Unit tests are an essential part of refactoring. Also I feel if the file upload size limit is increased or git repository can be used to for project submission, it will be great as well.

There was not enough clarification about meaning of the web service security. I thik "The ASMX service is secured" needs more clearification about what type of security is expected.