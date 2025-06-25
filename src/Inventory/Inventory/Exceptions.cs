namespace Inventory;

public class InventoryDomainException(string message) : Exception(message);

/***
 *
 * Perhaps someday this will use a shared library, or
 * perhaps it'll become InventoryDomainException so each
 * value stream / department / team has its own.
 * Who knows! ¯\_(ツ)_/¯
 *
 */
