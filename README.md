Anodyne
=======

Yet another framework for creating scalable applications with rich domain model and NoSQL.

It is designed to be highly pluggable and super-easy to use due to intensive use of DSL (fluent interfaces).
And if you're into F# (and functional programming in general) you can get some special comforts like Option, memoized functions, etc.

For example, typical application server configuration with Anodyne looks like this:

        public override void OnConfigure(IConfiguration c)
        {
            c.UseWindsorContainer();
            c.UseWindsorWcfServicePublisher();
            c.ForDataAccess().UseMongoDatabase(Configured.From.AppSettings("DatabaseServer", "DatabaseName"));

            c.OnStartupPerform<DataAccessConfiguration>();
            c.OnStartupPerform<WcfServicesRegistration>();
            c.OnStartupPerform<CommandConsumersRegistration>();
        }

See [documentation](https://anodyne.readthedocs.org) for more details.

Current Status
==============

Alpha (usable and used in production environments, but be prepared for many breaking changes).

Requirements
============

* .NET 4.0
* ASP.NET MVC 3 / 4 (for web application)
* Visual Studio 2010 SP1 for working with solution
* NuGet

Amazing projects in use
=======================

* [Castle Windsor](http://www.castleproject.org/)
* [Log4Net](http://logging.apache.org/log4net/)
* [MongoDb](http://www.mongodb.org/)
* [TopShelf](http://topshelf-project.com/)
* [NUnit](http://www.nunit.org/)
* [FakeItEasy](https://github.com/patrik-hagne/FakeItEasy)

...and many others.

License
=======

Licensed under Apache 2.0 License. Which means it's an open source project you can use in (almost) any of your projects (commercial and open-source alike).