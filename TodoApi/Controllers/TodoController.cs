using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Controllers
{//Routing is not case sensitive in ASP.NET
	[Route("api/[controller]")]
	public class TodoController : Controller
	{
		private readonly ITodoRepository _todoRepository;

		public TodoController(ITodoRepository todoRepository)
		{
			_todoRepository = todoRepository;
		}
		// Cannot have two HttpGet Methods >> tutorial doesn't tell you to replace it but add another?
		// This is an issue because the router won't know which one to execute and will throw the 500 
		// Same with GetById
		[HttpGet]
		public IEnumerable<TodoItem> GetAll()
		{
			return _todoRepository.GetAll();
		}

		[HttpGet("{id}", Name = "GetTodo")]
		public IActionResult GetById(long id)
		{
			var item = _todoRepository.Find(id);
			if (item == null)
			{
				return NotFound();
			}
			return new ObjectResult(item);

		}
		/// <summary>
		/// Creates a TodoItem
		/// </summary>
		/// <remarks>
		/// Not that the key is a GUID and not an integer.
		/// 
		///		POST /Todo
		///		{
		///			"key": "0e7ad584-7788-4ab1-95a6-ca0a5b444cbb",
		///			"name": "Item1",
		///			"isComplete": true
		///		}
		///		
		/// </remarks>
		/// <param name="item"></param>
		/// <returns>New Created Todo Item</returns>
		/// <response code="201">Returns the newly created item</response>
		/// <response code="400">If the item is null</response>
		
		[HttpPost]
		[ProducesResponseType(typeof(TodoItem), 201)]
		[ProducesResponseType(typeof(TodoItem), 400)]
		public IActionResult Create([FromBody, Required] TodoItem item)
		{
			if (item == null)
			{
				return BadRequest();
			}
			// this is named TodoItem on Swagger tutorial check to verify this is correct? 
			_todoRepository.Add(item);

			return CreatedAtRoute("GetTodo", new { id = item.Key }, item);
		}
		
		[HttpPut("{id}")]
		public IActionResult Update(long id, [FromBody] TodoItem item)
		{
			if (item == null || item.Key != id)
			{
				return BadRequest();
			}

			var todo = _todoRepository.Find(id);
			if (todo == null)
			{
				return NotFound();
			}

			todo.IsComplete = item.IsComplete;
			todo.Name = item.Name;

			_todoRepository.Update(todo);
			return new NoContentResult();
		}
		/// <summary>
		/// Deletes specific TodoItem
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		public IActionResult Delete(long id)
		{
			var todo = _todoRepository.Find(id);
			if (todo == null)
			{
				return NotFound();
			}

			_todoRepository.Remove(id);
			return new NoContentResult();
		}

	}
}
