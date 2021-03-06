const Joi = require('joi');

module.exports = {
    validateBody: (schema) => {
        return (req, res, next) => {
            const result = Joi.validate(req.body, schema);

            if (result.error) {
                return res.status(400).json(result.error);
            }

            if(!req.values) { req.values = {}; }
            req.values['body'] = result.value;
            next();
        };
    },

    schemas: {
        authSchema: Joi.object().keys({
            email: Joi.string().email().required(),
            password: Joi.string().required(),
            name: Joi.string(),
            gender: Joi.string().valid(['Male', 'Female']),
            birthday: Joi.date()
        }),
        loginSchema: Joi.object().keys({
            email: Joi.string().email().required(),
            password: Joi.string().required()
        }),
        reviewSchema: Joi.object().keys({
            id: Joi.number().integer(),
            appId: Joi.number().integer().required(),
            categoryName: Joi.string().required(),
            temporalContext: Joi.string().required().valid(['Intensive', 'Allocative']),
            spatialContext: Joi.string().required().valid(['Visiting', 'Traveling', 'Wandering']),
            socialContext: Joi.string().required().valid(['Constraining', 'Encouraging']),
            textReview: Joi.string().required()
        }),
        applicationSchema: Joi.object().keys( {
            name: Joi.string().required(),
            operatingSystem: Joi.string().required().valid(['Android', 'iOS'])
        }),
        userSchema: Joi.object().keys({
            email: Joi.string().email(),
            password: Joi.string(),
            name: Joi.string(),
            gender: Joi.string().valid(['Male', 'Female']),
            birthday: Joi.date()
        })
    }
}