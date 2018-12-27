using System;
using System.Collections.Generic;
using System.Linq;
using Marten.NodaTime.Testing.TestData;
using Marten.Testing;
using NodaTime;
using Shouldly;
using Xunit;

namespace Marten.NodaTime.Testing.Acceptance
{
    public class noda_time_acceptance : IntegratedFixture
    {
        [Fact]
        public void can_insert_document()
        {
            StoreOptions(_ => _.UseNodaTime());

            var testDoc = TargetWithDates.Generate();

            using (var session = theStore.OpenSession())
            {
                session.Insert(testDoc);
                session.SaveChanges();
            }

            using (var query = theStore.QuerySession())
            {
                var docFromDb = query.Query<TargetWithDates>().FirstOrDefault(d => d.Id == testDoc.Id);

                docFromDb.ShouldNotBeNull();
                docFromDb.Equals(testDoc).ShouldBeTrue();
            }
        }

        [Fact]
        public void can_query_document()
        {
            StoreOptions(_ => _.UseNodaTime());

            var dateTime = DateTime.UtcNow;
            var localDateTime = LocalDateTime.FromDateTime(dateTime);
            var testDoc = TargetWithDates.Generate(dateTime);

            using (var session = theStore.OpenSession())
            {
                session.Insert(testDoc);
                session.SaveChanges();
            }

            using (var query = theStore.QuerySession())
            {
                var results = new List<TargetWithDates>
                {
                    //query.Query<TargetWithDates>().FirstOrDefault(d => d.DateTime == dateTime),
                    query.Query<TargetWithDates>().FirstOrDefault(d => d.LocalDateTime == localDateTime),
                    query.Query<TargetWithDates>().FirstOrDefault(d => d.LocalDate == localDateTime.Date)
                };

                ////var q = query.Query<TargetWithDates>().Where(d => d.DateTime.Equals(dateTime));
                ////var q2 = query.Query<TargetWithDates>().Where(d => d.LocalDateTime.Equals(localDateTime));
                ////var res = q.ToList();
                ////var res2 = q2.ToList();

                results.ShouldAllBe(x => x.Equals(testDoc));
            }
        }
    }
}