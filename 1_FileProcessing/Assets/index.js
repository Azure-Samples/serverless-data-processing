const storage = require('azure-storage');
const tableName = 'TaxiSensorData';

module.exports = function (context, blobConent) {
    const connectionString = process.env['CosmosDBConnectionString'];
    const storageClient = storage.createTableService(connectionString);
    let index = 1;

    let entities = blobConent.trim().split('\n').map((json) => {
        const item = JSON.parse(json);

        let obj = {
            PartitionKey: { '_': "taxi_" + item.Name },
            RowKey: { '_': Date.now().toString() + "_" + (index++) }
        };

        for (var attrname in item) {
            if (attrname === 'Name') continue;
            obj[attrname] = { '_': item[attrname] };
        }

        return {
            item: obj
        };
    });

    storageClient.createTableIfNotExists(tableName, function (error, result, response) {
        if (!error) {
            insertEntities(entities);
        }
        else {
            context.log(error);
        }
    });

    context.done();

    function insertEntities(entities) {
        if (entities.length > 0) {
            let items = entities.splice(0, entities.length < 100 ? entities.length : 100);
            let batch = new storage.TableBatch();

            for (let i = 0; i < items.length; i++) {
                batch.insertEntity(items[i].item, { echoContent: true });
            }

            storageClient.executeBatch(tableName, batch, function (error, result, response) {
                if (!error) {
                    setTimeout(function () {
                        insertEntities(entities);
                    }, 1000);
                }
                else {
                    context.log(error);
                }
            });
        }
    }
};