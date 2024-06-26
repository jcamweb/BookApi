﻿using BookApi.Models;
using BookApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookApiRepository _repository;
        private readonly ILogger<Book> _logger;

        public BooksController(IBookApiRepository repository, ILogger<Book> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Devuelve lista de books 
        /// </summary>
        /// <returns>books</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _repository.GetAllAsync();
            return Ok(books);
        }

        /// <summary>
        /// Devuelve book por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>book</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _repository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        /// <summary>
        /// Actualiza book por id, title, author, genre, publishedyear
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Title"></param>
        /// <param name="Author"></param>
        /// <param name="Genre"></param>
        /// <param name="PublishedYear"></param>

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateAsync(book);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(book);
        }

        /// <summary>
        /// Crea book title, author, genre, publishedyear
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Author"></param>
        /// <param name="Genre"></param>
        /// <param name="PublishedYear"></param>
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repository.AddAsync(book);
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to save changes. ");
                _logger.LogError($"{nameof(PostBook)} No se ha podido crear book: {ex.Message}");
            }
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }


        /// <summary>
        /// Elimina book por id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return Ok(book);
        }

        /// <summary>
        /// Devuelve books coinciden en Title, Author o Genre con parámetro de búsqueda
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>books</returns>

        [HttpGet("search/{searchstring}")]
        public async Task<IActionResult> SearchBook(string searchstring)
        {
            var books = await _repository.SearchBookAsync(searchstring);
            if (books == null)
            {
                _logger.LogError($"Sin resultados en búsqueda de {searchstring}");
                return NotFound();
            }

            return Ok(books);
        }
       
    }
}

