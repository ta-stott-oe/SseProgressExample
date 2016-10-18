///<reference path="typings/jquery/jquery.d.ts" />
var Job;
(function (Job) {
    function DoJob() {
        return $.ajax("/api/job", {
            method: "PUT",
            data: { Foo: 4 }
        })
            .then(function (id) {
            var deferred = $.Deferred();
            try {
                var eventSource = new EventSource("/api/job/" + id + "/progress");
                eventSource.onmessage = function (message) {
                    deferred.notify(JSON.parse(message.data));
                };
            }
            catch (error) {
            }
            $.getJSON("/api/job/" + id)
                .then(function (result) { return deferred.resolve(result); })
                .fail(function (error) { return deferred.reject(error); });
            return deferred.promise();
        });
    }
    Job.DoJob = DoJob;
})(Job || (Job = {}));
//# sourceMappingURL=job.js.map