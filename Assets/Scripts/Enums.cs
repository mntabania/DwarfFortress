﻿//public enum BIOME{
//	OCEAN,
//	BEACH,
//	SCORCHED,
//	BARE,
//	TUNDRA,
//	SNOW,
//	TEMPERATE_DESERT,
//	SHRUBLAND,
//	TAIGA,
//	GRASSLAND,
//	TEMPERATE_DECIDUOUS_FOREST,
//	TEMPERATE_RAIN_FOREST,
//	SUBTROPICAL_DESERT,
//	TROPICAL_SEASONAL_FOREST,
//	TROPICAL_RAIN_FOREST,
//	FOREST,
//	JUNGLE,
//	SAVANNAH,
//	DESERT,
//}

public enum BIOMES{
	SNOW,
	TUNDRA,
	DESERT,
	GRASSLAND,
	WOODLAND,
	FOREST,
	BARE,
}

public enum ELEVATION{
	MOUNTAIN,
	WATER,
	PLAIN,
}

public enum EQUATOR_LINE{
	HORIZONTAL,
	VERTICAL,
	DIAGONAL_LEFT,
	DIAGONAL_RIGHT,
}

public enum CITY_TYPE{
	NORMAL,
	CAPITAL
}

public enum RACE{
	HUMANS,
	ELVES,
	MINGONS,
	CROMADS,
}

public enum EVENTS{
	EXPAND,
	DECLARE_WAR,
	DECLARE_PEACE,
	GENERATE_ARMY,
}

public enum PATH_DIRECTION{
	NORTH,
	NORTH_EAST,
	SOUTH_EAST,
	SOUTH,
	SOUTH_WEST,
	NORTH_WEST,
}

public enum JOB_TYPE{
//	DIPLOMAT,
//	SPY,
	FARMER,
	WOODSMAN,
	MINER,
	ALCHEMIST,
//	ARTISAN,
//	MERCHANT,
//	DEFENSE_GENERAL,
//	OFFENSE_GENERAL,
//	ARCHER,
//	MAGE,
	HUNTER,
	QUARRYMAN,
//	BRAWLER,
//	HEALER,
//	BUILDER,
//	ENCHANTER,
	PIONEER,
	NONE
}

public enum CHARACTER{
	LOGICAL,
	EMOTIONAL,
}

public enum GOALS{
	EXPAND,
	UPGRADE_CITY,
	UPGRADE_CITIZEN,
	PILFER_FUNDS,
	BEFRIEND_SOMEONE,
	UNDERMINE_SOMEONE,
	BECOME_KING,
	CREATE_NEW_CITIZEN,
}

public enum PUBLIC_IMAGE{
	RELIABLE,
	UNRELIABLE,
	LOYAL,
	BETRAYER,
	CHARISMATIC,
	UGLY,
	GENEROUS,
	SELFISH,
}

public enum REPRESENTATIVES{
	KING,
	MAYOR,
	CITIZENS,
}

public enum CITY_STATE{
	STARVATION,
	ABUNDANT,
}

public enum RESOURCE{
//	NONE,
	GOLD,
	FOOD,
	LUMBER,
	STONE,
	MANA,
	METAL,
}

public enum RESIDENCE{
	OUTSIDE,
	INSIDE
}

public enum RESOURCE_STATUS{
	SCARCE,
	NORMAL,
	ABUNDANT,
}
public enum BIOME_PRODUCE_TYPE{
	FARMING,
	HUNTING,
	WOOD,
	STONE,
	MANA,
	METAL,
	GOLD,
}

public enum DECISION{
	NEUTRAL,
	NICE,
	RUDE,
}
public enum LORD_PERSONALITY{
	TIT_FOR_TAT,
	EMOTIONAL,
	RATIONAL,
	NAIVE,
	HATER,
}

public enum LORD_INTERNAL_PERSONALITY_NEGATIVE{
//	NONE,
	CORRUPT,
	VIOLENT,
	TYRANT,
	DESTRUCTIVE,
}

public enum LORD_INTERNAL_PERSONALITY_POSITIVE{
//	NONE,
	GALLANT,
	GREEN_THUMB,
	MONEYMAKER,
	INSPIRING,
}

public enum LORD_EVENTS{
	TRADE,
	HELP,
	GIFT,
	COOPERATE1,
	COOPERATE2,
	NONE
}

public enum LORD_RELATIONSHIP{
	RIVAL,
	ENEMY,
	COLD,
	NEUTRAL,
	WARM,
	FRIEND,
	ALLY,
}

public enum GENERAL_CLASSIFICATION{
	OFFENSE,
	DEFENSE,
}

public enum MIGHT_TRAIT{
	UNDERDOG,
	BULLY,
	NORMAL
}

public enum RELATIONSHIP_TRAIT{
	WARMONGER,
	PEACEFUL,
	NORMAL
}

public enum WAR_REASONS{
	OPPORTUNIST,
	DEFENDER,
	COVETOUS,
	RACIST,
	ONE_TRUE_KING,
	SUSPICIOUS,
	TRADER,
	MONEY_GRUBBER,
	TRAITOR,
	IMPOTENT,
	PARANOID,
	SNEAKY,
	SCAVENGER,
	COMPETITIVE,
}

public enum PEACE_REASONS{
	DEFEATED,
	GOAL_REACHED,
	MANY_WARS,
	OFFER_ALLIANCE,
	ENEMY_OF_ENEMY,
}

public enum INTELLIGENCE{
	SMART,
	AVERAGE,
	SIMPLE,
}

public enum AGGRESSIVENESS{
	SUPER,
	MILD,
	LIGHT,
}

public enum BATTLE_MOVE{
	ATTACK,
	DEFEND,
}

public enum LORD_INSTRUCTIONS{
	INCREASE_ARMY,
	PROVIDE_HELP,
	CREATE_GENERALS,
	ATTACK,
	DEFEND,
}