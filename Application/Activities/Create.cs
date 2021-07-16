using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        //Commands do not return anything
        public class Command : IRequest
        {
            public Activity Activity { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            public DataContext _context { get; set; }
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
               _context.Activities.Add(request.Activity);
               await _context.SaveChangesAsync();

               return Unit.Value;               
            }
        }
    }
}