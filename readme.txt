Both of the 2 projects included in this starting-code package are basic ASP.NET Core MVC web apps. Both support basic CRUD operations on Movies as a primary entity.

However, MovieProductionCompany (MPC) has a richer data model with more relations and it uses Attribute routing to define a richer, more well-defined, URL hierarchy. Please note, however, that not all the links in this app are working -  there is a bit more work to do in order to take full advantage of the richer relationships in this data model but it is in a state that gives you the idea for how it all fits together.

In contrast, MovieStreamingService (MSS) manipulates simpler Movie entities that only have 1-to-1 relationship with Genres and another with ProductionCompanies. Also, it uses the default routing pattern. This app is only slightly modified from what we produced in class when we first started looking at DB-driven MVC apps.

Both a very simple and in no way reflect fully-featured MPC or MSS apps but nonetheless should suffice to implement the desired integration. Both should compile as is and, after running Update-Database, should run perfectly well but independently.

Your job in this problem analysis is to modify both of these apps to the extend needed to integrate them so that the notification of new movies are sent automaticaly from MPC to MSS when a new movie is added.