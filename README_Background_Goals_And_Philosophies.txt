Brain Dump of misc thoughts from kyle throughout set up of this is. 

--

There is a "run_everything.py" script at the root of the repo. This should 
spin up a bunch of Docker containers, including databases, .net backends, and Angular frontends. 
There should be sets of containers that simulated deployed sites. The idea is that from the 
single monorepo, you can see what it would look like when configured in "Ixom Mode", in "Bobcat Mode", 
etc.

-- 

See ctrl+shift+f for "fce5710e-1606-4ccb-9e5c-406cde46d156" an example of how API controllers 
specific to a given Client are gated to only work with a specific Client 

ClientConfigs 
- Envisioned the concept of a "ClientConfiguration" existing in both Angular and in .NET (i.e. frontend and backend)
- a "Client" is someone like Bobcat, Ixom, etc... maybe "Customer" or a better word for this 
- During deployment, this flag is set in a json config file that is read at runtime 
-- e.g. "run_deployment.py --ClientConfigCode Edison 
--- ^^^ invokes a Docker container deploy, and in the process sets "Edison" in a config file somewhere
--- at run time, we load that, and look up a "ClientConfiguration" using a switch statement  
- ctrl+shift+f for "c7e1e6ea-8da0-4dc8-932b-c1b8c0b460b6" to find a sample config (Bobcat)

---

Look into "Tree Shaking". This is a build process in which extra files that aren't used inside of 
a build can be thrown out.

For example, consider a case where we have TS/JS files for all clients: Ixom, Bobcat, and Edison. 
When each respectively client is built and declared, we should be able to set up "Tree Shaking" to throw 
out all files that aren't used in that particular deployment. (e.g. if we're deploying Ixom,
stuff that only Bobcat or only Edison used can theoretically be eliminated via tree shaking). Not exactly 
sure this mechanism would fit in but I'm sure it's possible.  