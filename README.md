AsyncSections
=============

Adds perceived speed from MVC4 AsyncConstollers via a new AsyncSectionController

<h3>What problem do AsyncSections solve?</h3>
<p>Some serverside functions take a while to complete, this can cause IIS thread starvation, which is addressed by MVC's AsyncController. Great! :-)</p>
<p>However, from a user's perspective nothing changes. They still have to wait x seconds for the page to respond and are are usually stuck on a white page that looks like the server isnt doing anything :-(</p>
<p>That's where AsyncSections come in! Imagine if you could seperate your view into sections which contain the results of your long running functions. Now you can! These are seperated from normal document rendering to allow the page to load first, then load in the results when they are ready.</p>

<h3>What are AsyncSections?</h3>
<p>AsyncSections are exactly that, they are standard MVC sections which are suffixed with "Async" and they dont get rendered to the page in the standard way.</p>
<p>The page is split into multiple render stages.</p>
<ol>
  <li>MasterPage is rendered up to the call to RenderBodyAsync()</li>
	<li>The body is rendered excluding anything in an AsyncSection</li>
	<li>A number of AsyncSections are rendered</li>
	<li>The last half of the MasterPage is rendered after the call to RenderBodyAsync()</li>
</ol>

![](https://raw.github.com/atlascode/asyncsections/master/AtlasCode.AsyncSections.Demos/Content/AsyncSectionsBasics.png?raw=true)

<p>In the @Html.ActionLink("example above", "Index", "Minimum"), you will see that the element in the Task1Async section will be rendered after the example footer</p>

<h3>When would I use an AsyncSection?</h3>
<p>Whenever you are doing something on the server that takes a few seconds but you dont want to make the user wait before allowing them to use the site. For example...</p>
<ul>
	<li>Processing an imported spreadsheet</li>
	<li>Waiting for 3rd party APIs to respond</li>
	<li>Doing some processing on all records in the database</li>
	<li>Generating documents</li>
	<li>Bulk functions like emailing all users or creating notification records</li>
	<li>Updating the database schema of the site using an EF 4.3 data migration :-)</li>
	<li>Use your imagination</li>
</ul>
<p>In theory, any page that accesses the database could use an AsyncSection, especially if it runs multiple different queries, like a KPI dashboard.</p>