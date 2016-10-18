///<reference path="typings/jquery/jquery.d.ts" />

module Job {
    export function DoJob(): JQueryPromise<any> {
        return $.ajax("api/job", {
                method: "PUT",
                data: { Foo: 4 }
            })
            .then(id => {
                return $.getJSON(`api/job/${id}`);
            });
    }
}