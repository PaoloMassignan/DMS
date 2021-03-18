using DMS.Kernel.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace DMS.Kernel.Test.Acceptance_Tests
{
    [Binding]
    class InitNodeSteps
    {
        private readonly InitNodeContext context;

        public InitNodeSteps(InitNodeContext context)
        {
            this.context = context;
        }

        [StepArgumentTransformation]
        public List<Message> GetMessages(Table table)
        {
            return table.Rows.Select(row => new Message
            {
                Node = row["Node"],
                Topic = row["Topic"],
                Data = row["Data"],
                SequenceId = Int32.Parse(row["SequenceId"]),
            }).ToList();
        }

        [StepArgumentTransformation]
        public List<DMSNode> GetNodes(Table table)
        {
            List<DMSNode> nodes = table.Rows.Select(row => new DMSNode(context.Container,row["NodeName"],0)).ToList();
            return nodes;
        }

        [StepArgumentTransformation]
        public DMSNode GetNode(string nodeName)
        {
            return context.Nodes.FirstOrDefault(x=>x.Name == nodeName);
        }


        [Given(@"Nodes of the cluster are")]
        public void GivenNodesOfTheClusterAre(List<DMSNode> nodes)
        {
            nodes.ForEach(x =>
            {
                context.AddMockForRemoteNode(x.Name);
            });
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i == j)
                        continue;
                    nodes[i].AddRemoteNode(nodes[j].Name,"");
                }
            }

            context.Nodes = nodes;
            foreach (var k in nodes)
            {
                var mem = k.Communication as InMemoryCommunication;
                mem.Nodes = nodes;
            }
        }


        [Given(@"messages in '(.*)' are")]
        public void GivenMessagesInAre(string nodeName, List<Message> messages)
        {
            context.RegisterMessagesFor(nodeName, messages);
        }

        [When(@"'(.*)' send UpdateRequest to '(.*)'")]
        public void WhenSendUpdateRequestTo(string from, string to)
        {
            DMSNode node = context.Nodes.FirstOrDefault(x => x.Name == from);
            UpdateRequest updateRequest = node.CreateUpdateRequest(to);
            node.SendUpdateRequest(updateRequest);
        }

        [Then(@"the response should be")]
        public void ThenTheResponseShouldBe(List<Message> messages)
        {
            context.Communication.ElaborateAllCommands();
            context.Communication.LastUpdateResponse.Messages.Should().BeEquivalentTo(messages);
        }

        [Given(@"HeartBeat Frequency is (.*)")]
        public void GivenHeartBeatFrequencyIs(int f)
        {
            context.Nodes.ForEach(x => x.HeartBeatFrequency = f);
        }

        [Given(@"'(.*)' write (.*) events/second")]
        public void GivenWriteEventsSecond(DMSNode node, int f)
        {
            context.NodeEventFrequency[node.Name] = f;
        }

        [Given(@"'(.*)' starts at (.*)")]
        public void GivenStartsAt(DMSNode node, int start)
        {
            context.Simulations.Add(new SimulationEvent() {Type = SimulationEvent.SimulationEventType.Start, Node = node, T = start });
        }

        [Given(@"'(.*)' stops at (.*)")]
        public void GivenStopssAt(DMSNode node, int start)
        {
            context.Simulations.Add(new SimulationEvent() { Type = SimulationEvent.SimulationEventType.Stop, Node = node, T = start });
        }

        [Given(@"Simulation ends at (.*)")]
        public void GivenSimulationEndsAt(int end)
        {
            context.StartSimulation(end);
        }

        [Then(@"At (.*) nodes are aligned")]
        public void ThenAtNodesAreAligned(int T)
        {
            context.ContinueSimulation(T);
            context.Nodes.CompareMessages().Should().BeTrue();
        }

        [Then(@"Sequence are respected")]
        public void ThenSequenceAreRespected()
        {
            context.Nodes.SequenceIsRespected().Should().BeTrue();
        }

        [Then(@"Messages are not duplicated")]
        public void ThenMessagesAreNotDuplicated()
        {
            foreach (var k in context.Nodes)
            {
                k.Persistence.GetAllMessages().GroupBy(x => new { x.SequenceId, x.Node }).Count().Should().Be(k.Persistence.GetAllMessages().Count);
            }

        }

    }
}
