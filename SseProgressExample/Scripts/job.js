///<reference path="typings/jquery/jquery.d.ts" />
var Job;
(function (Job) {
    function DoJob() {
        // Call PUT method to start job 
        return $.ajax("/api/job", {
            method: "PUT",
            data: { Foo: 4 } // Job parameters
        })
            .then(function (id) {
            // Create deferred object explicitly so we can send progress updates
            var deferred = $.Deferred();
            // Point EventSource at progress method for this job
            try {
                var eventSource = new EventSource("/api/job/" + id + "/progress");
                eventSource.onmessage = function (message) {
                    deferred.notify(JSON.parse(message.data)); // Messages have "data" property which in our case is JSON
                };
            }
            catch (error) {
            }
            // Call GET method to get job result and wire it up to deferred object
            $.getJSON("/api/job/" + id)
                .then(function (result) { return deferred.resolve(result); })
                .fail(function (error) { return deferred.reject(error); });
            return deferred.promise();
        });
    }
    Job.DoJob = DoJob;
})(Job || (Job = {}));
