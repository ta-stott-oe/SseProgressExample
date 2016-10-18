///<reference path="typings/jquery/jquery.d.ts" />
var Job;
(function (Job) {
    function DoJob() {
        return $.ajax("api/job", {
            method: "PUT",
            data: { Foo: 4 }
        })
            .then(function (id) {
            return $.getJSON("api/job/" + id);
        });
    }
    Job.DoJob = DoJob;
})(Job || (Job = {}));
//# sourceMappingURL=job.js.map